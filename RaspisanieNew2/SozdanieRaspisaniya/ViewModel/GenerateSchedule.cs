﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Raspisanie.Models;

namespace SozdanieRaspisaniya.ViewModel
{
    class GenerateSchedule
    {

        // Фитнесс функции
        static class FitnessFunctions
        {
            public static int GroupWindowPenalty = 10;//штраф за окно у группы
            public static int LateLessonPenalty = 1;//штраф за позднюю пару

            public static int LatesetHour = 4;//максимальный час, когда удобно проводить пары

            // Штраф за окна
            public static int Windows(Plan plan)
            {
                var res = 0;

                for (int day = 0; day < Plan.DaysPerWeek; day++)
                {
                    var groupHasLessions = new HashSet<int>();

                    for (int hour = 0; hour < Plan.HoursPerDay; hour++)
                    {
                        foreach (var pair in plan.HourPlans[day, hour].GroupInform)
                        {
                            var group = pair.Key;
                            var teacher = pair.Value;
                            if (groupHasLessions.Contains(group) && !plan.HourPlans[day, hour - 1].GroupInform.ContainsKey(group))
                                res += GroupWindowPenalty;

                            groupHasLessions.Add(group);
                        }
                    }
                }
                return res;
            }

            // Штраф за поздние пары
            public static int LateLesson(Plan plan)
            {
                var res = 0;
                foreach (var pair in plan.GetLessons())
                    if (pair.pairInfo.Pair > LatesetHour)
                        res += LateLessonPenalty;

                return res;
            }
        }

        // Решатель (генетический алгоритм)
        class Solver
        {
            public int MaxIterations = 1000;
            public int PopulationCount = 100;//должно делиться на 4

            public List<Func<Plan, int>> FitnessFunctions = new List<Func<Plan, int>>();
            public int Fitness(Plan plan)
            {
                var res = 0;

                foreach (var f in FitnessFunctions)
                    res += f(plan);

                return res;
            }

            public Plan Solve(List<Lesson> pairs)
            {
                //создаем популяцию
                var pop = new Population(pairs, PopulationCount);
                if (pop.Count == 0)
                    throw new Exception("Can not create any plan");
                //
                var count = MaxIterations;
                while (count-- > 0)
                {
                    //считаем фитнесс функцию для всех планов
                    pop.ForEach(p => p.FitnessValue = Fitness(p));
                    //сортруем популяцию по фитнесс функции
                    pop.Sort((p1, p2) => p1.FitnessValue.CompareTo(p2.FitnessValue));
                    //найден идеальный план?
                    if (pop[0].FitnessValue == 0)
                        return pop[0];
                    //отбираем 25% лучших планов
                    pop.RemoveRange(pop.Count / 4, pop.Count - pop.Count / 4);
                    //от каждого создаем трех потомков с мутациями
                    var c = pop.Count;
                    for (int i = 0; i < c; i++)
                    {
                        pop.AddChildOfParent(pop[i]);
                        pop.AddChildOfParent(pop[i]);
                        pop.AddChildOfParent(pop[i]);
                    }
                }

                //считаем фитнесс функцию для всех планов
                pop.ForEach(p => p.FitnessValue = Fitness(p));
                //сортруем популяцию по фитнесс функции
                pop.Sort((p1, p2) => p1.FitnessValue.CompareTo(p2.FitnessValue));

                //возвращаем лучший план
                return pop[0];
            }
        }

        // Популяция планов
        class Population : List<Plan>
        {
            public Population(List<Lesson> pairs, int count)
            {
                var maxIterations = count * 2;

                do
                {
                    var plan = new Plan();
                    if (plan.Init(pairs))
                        Add(plan);
                } while (maxIterations-- > 0 && Count < count);
            }

            public bool AddChildOfParent(Plan parent)
            {
                int maxIterations = 10;
                do
                {
                    var plan = new Plan();
                    if (plan.Init(parent))
                    {
                        Add(plan);
                        return true;
                    }
                } while (maxIterations-- > 0);
                return false;
            }
        }

        //План занятий
        class Plan
        {
            public static int DaysPerWeek = 6;//6 учебных дня в неделю
            public static int HoursPerDay = 9;//до 9 пар в день

            static Random rnd = new Random(3);
            // План по дням (первый индекс) и часам (второй индекс)
            public HourPlan[,] HourPlans = new HourPlan[DaysPerWeek, HoursPerDay];

            public int FitnessValue { get; internal set; }

            public bool AddLesson(Lesson les)
            {
                return HourPlans[(int)les.pairInfo.Day, les.pairInfo.Pair].AddLesson(les.dropInfo.Group.Single().CodeOfGroup, les.dropInfo);
            }

            public void RemoveLesson(Lesson les)
            {
                HourPlans[(int)les.pairInfo.Day, les.pairInfo.Pair].RemoveLesson(les.dropInfo.Group.Single().CodeOfGroup);
            }

            // Добавить группу на любой день и любой час
            public bool AddToAnyDayAndHour(int group, DropInformation dropInfo)
            {
                int maxIterations = 30;
                do
                {
                    var day = (byte)rnd.Next(DaysPerWeek);
                    if (AddToAnyHour(day, group, dropInfo))
                        return true;
                } while (maxIterations-- > 0);

                return false;//не смогли добавить никуда
            }

            // Добавить группу на любой час
            bool AddToAnyHour(int day, int group, DropInformation dropInfo)
            {
                for (int hour = 0; hour < HoursPerDay; hour++)
                {
                    var les = new Lesson(new PairInfo(hour,(DayOfWeek)day), dropInfo);
                    if (AddLesson(les))
                        return true;
                }
                return false;//нет свободных часов в этот день
            }

            // Создание плана по списку пар
            public bool Init(List<Lesson> pairs)
            {
                for (int i = 0; i < HoursPerDay; i++)
                    for (int j = 0; j < DaysPerWeek; j++)
                        HourPlans[j, i] = new HourPlan();

                foreach (var p in pairs)
                    if (!AddToAnyDayAndHour(p.dropInfo.Group.Single().CodeOfGroup, p.dropInfo))
                        return false;
                return true;
            }

            // Создание наследника с мутацией
            public bool Init(Plan parent)
            {
                //копируем предка
                for (int i = 0; i < HoursPerDay; i++)
                    for (int j = 0; j < DaysPerWeek; j++)
                        HourPlans[j, i] = parent.HourPlans[j, i].Clone();

                //выбираем два случайных дня
                var day1 = (byte)rnd.Next(DaysPerWeek);
                var day2 = (byte)rnd.Next(DaysPerWeek);

                //находим пары в эти дни
                var pairs1 = GetLessonsOfDay(day1).ToList();
                var pairs2 = GetLessonsOfDay(day2).ToList();

                //выбираем случайные пары
                if (pairs1.Count == 0 || pairs2.Count == 0) return false;
                var pair1 = pairs1[rnd.Next(pairs1.Count)];
                var pair2 = pairs2[rnd.Next(pairs2.Count)];

                //создаем мутацию - переставляем случайные пары местами
                RemoveLesson(pair1);//удаляем
                RemoveLesson(pair2);//удаляем
                var res1 = AddToAnyHour((int)pair2.pairInfo.Day, pair1.dropInfo.Group.Single().CodeOfGroup, pair1.dropInfo);//вставляем в случайное место
                var res2 = AddToAnyHour((int)pair1.pairInfo.Day, pair2.dropInfo.Group.Single().CodeOfGroup, pair2.dropInfo);//вставляем в случайное место
                return res1 && res2;
            }

            public IEnumerable<Lesson> GetLessonsOfDay(int day)
            {
                for (int hour = 0; hour < HoursPerDay; hour++)
                    foreach (var p in HourPlans[day, hour].GroupInform)
                        yield return new Lesson(new PairInfo(hour,(DayOfWeek)day), p.Value);
            }

            public IEnumerable<Lesson> GetLessons()
            {
                for (int day = 0; day < DaysPerWeek; day++)
                    for (int hour = 0; hour < HoursPerDay; hour++)
                        foreach (var p in HourPlans[day, hour].GroupInform)
                            yield return new Lesson(new PairInfo(hour,(DayOfWeek)day), p.Value);
            }

            //public override string ToString()
            //{
            //    var sb = new StringBuilder();
            //    for (byte day = 0; day < Plan.DaysPerWeek; day++)
            //    {
            //        sb.AppendFormat("Day {0}\r\n", day);
            //        for (byte hour = 0; hour < Plan.HoursPerDay; hour++)
            //        {
            //            sb.AppendFormat("Hour {0}: ", hour);
            //            foreach (var p in HourPlans[day, hour].GroupToTeacher)
            //                sb.AppendFormat("Gr-Tch: {0}-{1} ", p.Key, p.Value);
            //            sb.AppendLine();
            //        }
            //    }

            //    sb.AppendFormat("Fitness: {0}\r\n", FitnessValue);

            //    return sb.ToString();
            //}

        }

        //План на час
        class HourPlan
        {
            public Dictionary<int, DropInformation> GroupInform = new Dictionary<int, DropInformation>();

            public bool AddLesson(int group, DropInformation dropInfo)
            {
                if (GroupInform.ContainsKey(group))
                    return false;//в этот час уже есть пара у группы //у препода или 

                GroupInform[group] = dropInfo;
                return true;
            }

            public void RemoveLesson(int group)
            {
                GroupInform.Remove(group);
            }

            public HourPlan Clone()
            {
                var res = new HourPlan();
                res.GroupInform = new Dictionary<int, DropInformation>(GroupInform);
                return res;
            }
        }

        //пара
        class Lesson
        {
            public PairInfo pairInfo;
            public DropInformation dropInfo;

            public Lesson(DropInformation dropInformation)
            {
                dropInfo = dropInformation;
            }

            public Lesson(PairInfo pairinfo, DropInformation dropInformation) : this(dropInformation)
            {
                pairInfo = pairinfo;
            }

        }
    }
}
