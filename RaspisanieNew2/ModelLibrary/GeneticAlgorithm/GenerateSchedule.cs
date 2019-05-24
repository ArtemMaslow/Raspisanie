using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Models.GeneticAlgorithm
{
    public class GenerateSchedule
    {
        // Фитнесс функции
        public static class FitnessFunctions
        {
            public static int GroupWindowPenalty = 25;//штраф за окно у группы
            public static int LateLessonPenalty = 1;//штраф за позднюю пару
            public static int CountPairPenalty = 50;//штраф за превышение кол-ва пар в день 
            public static int CountLecturePairPenalty = 25;//штраф за превышение кол-ва лекций в день
            public static int CountMovePenalty = 20;//штраф за более чем один переход из 5 корпуса в другие и наоборот
            public static int AlwaysPairPenalty = 3;//штраф за каждую пару

            public static int UnusualDay = 5; // Суббота, день когда кол-во пар отличается от обычного
            public static int CountPair = 5;//максимальное кол-во пар в день
            public static int CountLecturePair = 3;//максимальное кол-вол лекций в день
            public static int LatesetHour = 4;//максимальный час, когда удобно проводить пары
            public static int CountMove = 1;//максимальное число переходов из 5 корпуса в другие и наоборот

            public static GroupsAndSubjects[] gas;
            public static TeachersAndSubjects[] tas;
            public static string[] Specifics;

            // Штраф за окна-----
            public static int Windows(Plan plan)
            {
                bool hadPairs = false;
                bool isWindowOrEmpty = false;
                bool hadPairsTwo = false;
                bool isWindowOrEmptyTwo = false;

                var res = 0;
                for (int i = 0; i < gas.Length; i++)
                {
                    for (int day = 0; day < Plan.DaysPerWeek; day++)
                    {
                        var count = day == UnusualDay ? Plan.SaturdayHoursPerDay : Plan.HoursPerDay;
                        isWindowOrEmpty = false;
                        hadPairs = false;
                        hadPairsTwo = false;
                        isWindowOrEmptyTwo = false;

                        for (int hour = 0; hour < count; hour++)
                        {
                            if (plan.HourPlans[day, hour].GroupInform.ContainsKey(gas[i].Group.CodeOfGroup))
                            {
                                if (hadPairs && isWindowOrEmpty)
                                {
                                    res += GroupWindowPenalty;
                                }
                                hadPairs = true;
                                isWindowOrEmpty = false;
                            }
                            else
                            {
                                isWindowOrEmpty = true;
                            }

                            Dictionary<int, Lesson> GroupInformGeneral = plan.HourPlans[day, hour].GroupInform
                                .Where(g => g.Value.dropInfo.Ndindex == 0)
                                .ToDictionary(g => g.Key, g => g.Value);

                            foreach (var item in plan.HourPlans[day, hour].GroupInformTwo)
                            {
                                if (!GroupInformGeneral.ContainsKey(item.Key))
                                    GroupInformGeneral.Add(item.Key, item.Value);

                            }

                            if (GroupInformGeneral.ContainsKey(gas[i].Group.CodeOfGroup))
                            {
                                if (hadPairsTwo && isWindowOrEmptyTwo)
                                {
                                    res += GroupWindowPenalty;
                                }

                                hadPairsTwo = true;
                                isWindowOrEmptyTwo = false;
                            }
                            else
                            {
                                isWindowOrEmptyTwo = true;
                            }
                        }
                    }
                }
                return res;
            }

            //штраф за превышение кол-ва пар в день у группы+++
            public static int CountPairGroups(Plan plan)
            {
                var res = 0;
                for (int i = 0; i < gas.Length; i++)
                {
                    for (int day = 0; day < Plan.DaysPerWeek; day++)
                    {
                        var groupCountLessons = new HashSet<DropInformation>();
                        var groupCountLessonsNum = new HashSet<DropInformation>();
                        var groupCountLessonsDenum = new HashSet<DropInformation>();

                        var count = day == UnusualDay ? Plan.SaturdayHoursPerDay : Plan.HoursPerDay;

                        for (int hour = 0; hour < count; hour++)
                        {
                            foreach (var pair in plan.HourPlans[day, hour].GroupInform)
                            {
                                if (gas[i].Group.CodeOfGroup == pair.Key)
                                {
                                    if (pair.Value.dropInfo.Ndindex == 0)
                                    {
                                        groupCountLessons.Add(pair.Value.dropInfo);
                                    }
                                    if (pair.Value.dropInfo.Ndindex == 1)
                                    {
                                        groupCountLessonsNum.Add(pair.Value.dropInfo);
                                    }
                                }
                            }

                            foreach (var pair in plan.HourPlans[day, hour].GroupInformTwo)
                            {
                                if (pair.Value.dropInfoTwo.Ndindex == -1)
                                {
                                    groupCountLessonsDenum.Add(pair.Value.dropInfo);
                                }
                            }
                        }

                        int groupCountLessonsGeneral = groupCountLessons.Count;
                        int groupCountLessonsNumWeek = groupCountLessons.Count + groupCountLessonsNum.Count;
                        int groupCountLessonsDenumWeek = groupCountLessons.Count + groupCountLessonsDenum.Count;

                        if (groupCountLessonsGeneral > CountPair
                            || groupCountLessonsNumWeek > CountPair
                            || groupCountLessonsDenumWeek > CountPair
                            || groupCountLessons.Count == 0)
                            res += CountPairPenalty;

                        //res += AlwaysPairPenalty * groupCountLessions.Count;
                    }
                }
                return res;
            }

            //штраф за превышение кол-ва пар в день у преподавателя+++
            public static int CountPairTeachers(Plan plan)
            {
                var res = 0;
                for (int i = 0; i < tas.Length; i++)
                {
                    for (int day = 0; day < Plan.DaysPerWeek; day++)
                    {
                        var teacherCountLessons = new HashSet<DropInformation>();
                        var teacherCountLessonsNum = new HashSet<DropInformation>();
                        var teacherCountLessonsDenum = new HashSet<DropInformation>();

                        var count = day == UnusualDay ? Plan.SaturdayHoursPerDay : Plan.HoursPerDay;

                        for (int hour = 0; hour < count; hour++)
                        {
                            foreach (var pair in plan.HourPlans[day, hour].TeacherInform)
                            {
                                if (tas[i].Teacher.CodeOfTeacher == pair.Key)
                                {
                                    if (pair.Value.dropInfo.Ndindex == 0)
                                    {
                                        teacherCountLessons.Add(pair.Value.dropInfo);
                                    }
                                    if (pair.Value.dropInfo.Ndindex == 1)
                                    {
                                        teacherCountLessonsNum.Add(pair.Value.dropInfo);
                                    }
                                }
                            }

                            foreach (var pair in plan.HourPlans[day, hour].TeacherInformTwo)
                            {
                                if (pair.Value.dropInfoTwo.Ndindex == -1)
                                {
                                    teacherCountLessonsDenum.Add(pair.Value.dropInfoTwo);
                                }
                            }
                        }
                        int teacherCountLessonsGeneral = teacherCountLessons.Count;
                        int teacherCountLessonsNumWeek = teacherCountLessons.Count + teacherCountLessonsNum.Count;
                        int teacherCountLessonsDenumWeek = teacherCountLessons.Count + teacherCountLessonsDenum.Count;

                        if (teacherCountLessonsGeneral > CountPair
                            || teacherCountLessonsNumWeek > CountPair
                            || teacherCountLessonsDenumWeek > CountPair)
                            res += CountPairPenalty;
                    }
                }
                return res;
            }

            //штраф за превышение кол-ва лекций в день у группы+++
            public static int CountLecturePairGroups(Plan plan)
            {
                var res = 0;
                for (int i = 0; i < gas.Length; i++)
                {
                    for (int day = 0; day < Plan.DaysPerWeek; day++)
                    {
                        var groupLectureCountLessons = new HashSet<DropInformation>();
                        var groupLectureCountLessonsNum = new HashSet<DropInformation>();
                        var groupLectureCountLessonsDenum = new HashSet<DropInformation>();

                        var count = day == UnusualDay ? Plan.SaturdayHoursPerDay : Plan.HoursPerDay;

                        for (int hour = 0; hour < count; hour++)
                        {
                            foreach (var pair in plan.HourPlans[day, hour].GroupInform)
                            {
                                if (pair.Value.dropInfo.Specifics.Equals(Specifics[0]))
                                {
                                    if (gas[i].Group.CodeOfGroup == pair.Key)
                                    {
                                        if (pair.Value.dropInfo.Ndindex == 0)
                                        {
                                            groupLectureCountLessons.Add(pair.Value.dropInfo);
                                        }
                                        if (pair.Value.dropInfo.Ndindex == 1)
                                        {
                                            groupLectureCountLessonsNum.Add(pair.Value.dropInfo);
                                        }
                                    }
                                }
                            }

                            foreach (var pair in plan.HourPlans[day, hour].GroupInformTwo)
                            {
                                if (pair.Value.dropInfoTwo.Specifics.Equals(Specifics[0]))
                                {
                                    if (gas[i].Group.CodeOfGroup == pair.Key)
                                    {
                                        if (pair.Value.dropInfoTwo.Ndindex == -1)
                                        {
                                            groupLectureCountLessonsDenum.Add(pair.Value.dropInfoTwo);
                                        }
                                    }
                                }
                            }

                        }

                        int groupCountLectureLessonsGeneral = groupLectureCountLessons.Count;
                        int groupCountLectureLessonsNumWeek = groupLectureCountLessons.Count + groupLectureCountLessonsNum.Count;
                        int groupCountLectureLessonsDenumWeek = groupLectureCountLessons.Count + groupLectureCountLessonsDenum.Count;

                        if (groupCountLectureLessonsGeneral > CountLecturePair
                            || groupCountLectureLessonsNumWeek > CountLecturePair
                            || groupCountLectureLessonsDenumWeek > CountLecturePair)
                            res += CountLecturePairPenalty;
                    }
                }
                return res;
            }

            //штраф за превышение кол-ва лекций в день у преподавателя+++
            public static int CountLecturePairTeachers(Plan plan)
            {
                var res = 0;
                for (int i = 0; i < tas.Length; i++)
                {
                    for (int day = 0; day < Plan.DaysPerWeek; day++)
                    {
                        var teacherLectureCountLessons = new HashSet<DropInformation>();
                        var teacherLectureCountLessonsNum = new HashSet<DropInformation>();
                        var teacherLectureCountLessonsDenum = new HashSet<DropInformation>();

                        var count = day == UnusualDay ? Plan.SaturdayHoursPerDay : Plan.HoursPerDay;

                        for (int hour = 0; hour < count; hour++)
                        {
                            foreach (var pair in plan.HourPlans[day, hour].TeacherInform)
                            {
                                if (pair.Value.dropInfo.Specifics.Equals(Specifics[0]))
                                {
                                    if (tas[i].Teacher.CodeOfTeacher == pair.Key)
                                    {
                                        if (pair.Value.dropInfo.Ndindex == 0)
                                        {
                                            teacherLectureCountLessons.Add(pair.Value.dropInfo);
                                        }
                                        if (pair.Value.dropInfo.Ndindex == 1)
                                        {
                                            teacherLectureCountLessonsNum.Add(pair.Value.dropInfo);
                                        }
                                    }
                                }
                            }

                            foreach (var pair in plan.HourPlans[day, hour].TeacherInformTwo)
                            {
                                if (pair.Value.dropInfoTwo.Specifics.Equals(Specifics[0]))
                                {
                                    if (tas[i].Teacher.CodeOfTeacher == pair.Key)
                                    {
                                        if (pair.Value.dropInfoTwo.Ndindex == -1)
                                        {
                                            teacherLectureCountLessonsDenum.Add(pair.Value.dropInfoTwo);
                                        }
                                    }
                                }
                            }
                        }

                        int teacherCountLectureLessonsGeneral = teacherLectureCountLessons.Count;
                        int teacherCountLectureLessonsNumWeek = teacherLectureCountLessons.Count + teacherLectureCountLessonsNum.Count;
                        int teacherCountLectureLessonsDenumWeek = teacherLectureCountLessons.Count + teacherLectureCountLessonsDenum.Count;

                        if (teacherCountLectureLessonsGeneral > CountLecturePair
                            || teacherCountLectureLessonsNumWeek > CountLecturePair
                            || teacherCountLectureLessonsDenumWeek > CountLecturePair)
                            res += CountLecturePairPenalty;
                    }
                }
                return res;
            }

            //штраф за более чем один переход из 5 корпуса в другие и наоборот------
            //public static int CountMoveFromFiveHousingToOtherAndConversely(Plan plan)
            //{
            //    var res = 0;
            //    var count = 0;
            //    for (int day = 0; day < Plan.DaysPerWeek; day++)
            //    {
            //        for (int hour = 1; hour < Plan.HoursPerDay; hour++)
            //        {
            //            foreach (var pair in plan.HourPlans[day, hour].ClassroomInform)
            //            {
            //                var housing = pair.Value.NumberOfClassroom.NumberOfClassroom.Split('/');
            //                //более чем один элемент
            //                var temp = plan.HourPlans[day, hour - 1].ClassroomInform.Single().Value.NumberOfClassroom.NumberOfClassroom;
            //                var nextHousing = temp.Split('/');

            //                if ((housing[0].Equals("5") && !nextHousing[0].Equals("5")) || (!housing[0].Equals("5") && nextHousing[0].Equals("5")))
            //                {
            //                    count++;
            //                }
            //            }
            //        }
            //        if (count > CountMove)
            //        {
            //            res += CountMovePenalty;
            //        }
            //    }
            //    return res;
            //}

            // Штраф за поздние пары-----
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
        public class Solver
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
                    //сортируем популяцию по фитнесс функции
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
                Console.WriteLine(count);
                //считаем фитнесс функцию для всех планов
                pop.ForEach(p => p.FitnessValue = Fitness(p));
                //сортруем популяцию по фитнесс функции
                pop.Sort((p1, p2) => p1.FitnessValue.CompareTo(p2.FitnessValue));

                //возвращаем лучший план
                return pop[0];
            }
        }

        // Популяция планов
        public class Population : List<Plan>
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
        public class Plan
        {
            public static int DaysPerWeek = 6;//6 учебных дня в неделю
            public static int HoursPerDay = 6;//до 6 пар в день
            public static int SaturdayHoursPerDay = 3;//до 3 пар в субботу

            static Random rnd = new Random(3);
            // План по дням (первый индекс) и часам (второй индекс)
            public HourPlan[,] HourPlans = new HourPlan[DaysPerWeek, HoursPerDay];

            public int FitnessValue { get; internal set; }

            public bool AddLesson(Lesson les)
            {
                return HourPlans[(int)les.pairInfo.Day, les.pairInfo.Pair].AddLesson(les);
            }

            public void RemoveLesson(Lesson les)
            {
                HourPlans[(int)les.pairInfo.Day, les.pairInfo.Pair].RemoveLesson(les);
            }

            // Добавить группу на любой день и любой час
            public bool AddToAnyDayAndHour(Lesson lesson)
            {
                int maxIterations = 30;
                do
                {
                    var day = (byte)rnd.Next(DaysPerWeek);
                    if (AddToAnyHour(day, lesson))
                        return true;
                } while (maxIterations-- > 0);

                return false;//не смогли добавить никуда
            }

            // Добавить группу на любой час
            bool AddToAnyHour(int day, Lesson lesson)
            {
                var count = day == 5 ? SaturdayHoursPerDay : HoursPerDay;

                for (int hour = 0; hour < count; hour++)
                {
                    var les = new Lesson(new PairInfo(hour, (DayOfWeek)day), lesson.dropInfo, lesson.dropInfoTwo);
                    if (AddLesson(les))
                        return true;
                }
                return false;//нет свободных часов в этот день
            }

            // Создание плана по списку пар
            public bool Init(List<Lesson> pairs)
            {
                for (int i = 0; i < HoursPerDay; i++)
                {
                    for (int j = 0; j < DaysPerWeek; j++)
                        HourPlans[j, i] = new HourPlan();
                }
                foreach (var p in pairs)
                {
                    if (!AddToAnyDayAndHour(p))
                        return false;
                }
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
                //bool dayresult = false;

                //if (pair1.dropInfo.Ndindex == 0 && pair2.dropInfo.Ndindex == 0)
                //{
                //    var dayresult2 = false;
                //    var dayresult1 = false;

                //    foreach (var teacher in FitnessFunctions.tas)
                //    {
                //        if (pair1.dropInfo.Teacher.CodeOfTeacher == teacher.Teacher.CodeOfTeacher)
                //        {
                //            if (teacher.DayList.Contains(pair2.pairInfo.Day))
                //            {
                //                dayresult2 = true;
                //            }
                //        }

                //        if (pair2.dropInfo.Teacher.CodeOfTeacher == teacher.Teacher.CodeOfTeacher)
                //        {
                //            if (teacher.DayList.Contains(pair1.pairInfo.Day))
                //            {
                //                dayresult1 = true;
                //            }
                //        }
                //        dayresult = dayresult1 && dayresult2;
                //    }
                //}
                //else if (pair1.dropInfo.Ndindex == 0 && pair2.dropInfo.Ndindex == 1)
                //{
                //    var dayresult2 = false;
                //    var dayresult1 = false;
                //    var dayresult3 = false;

                //    foreach (var teacher in FitnessFunctions.tas)
                //    {
                //        if (pair1.dropInfo.Teacher.CodeOfTeacher == teacher.Teacher.CodeOfTeacher)
                //        {
                //            if (teacher.DayList.Contains(pair2.pairInfo.Day))
                //            {
                //                dayresult2 = true;
                //            }
                //        }

                //        if (pair2.dropInfo.Teacher.CodeOfTeacher == teacher.Teacher.CodeOfTeacher)
                //        {
                //            if (teacher.DayList.Contains(pair1.pairInfo.Day))
                //            {
                //                dayresult1 = true;
                //            }
                //        }
                //        if (pair2.dropInfoTwo != null)
                //        {
                //            if (pair2.dropInfoTwo.Teacher.CodeOfTeacher == teacher.Teacher.CodeOfTeacher)
                //            {
                //                if (teacher.DayList.Contains(pair2.pairInfo.Day))
                //                {
                //                    dayresult1 = true;
                //                }
                //            }
                //        }
                //        else
                //        {
                //            dayresult3 = true;
                //        }
                //        dayresult = dayresult1 && dayresult2 && dayresult3;
                //    }
                //}
                //else if (pair1.dropInfo.Ndindex == 1 && pair2.dropInfo.Ndindex == 0)
                //{
                //    var dayresult2 = false;
                //    var dayresult1 = false;
                //    var dayresult3 = false;
                //    foreach (var teacher in FitnessFunctions.tas)
                //    {
                //        if (pair1.dropInfo.Teacher.CodeOfTeacher == teacher.Teacher.CodeOfTeacher)
                //        {
                //            if (teacher.DayList.Contains(pair2.pairInfo.Day))
                //            {
                //                dayresult2 = true;
                //            }
                //        }

                //        if (pair2.dropInfo.Teacher.CodeOfTeacher == teacher.Teacher.CodeOfTeacher)
                //        {
                //            if (teacher.DayList.Contains(pair1.pairInfo.Day))
                //            {
                //                dayresult1 = true;
                //            }
                //        }

                //    }
                //}

                //if (dayresult)
                //{
                //создаем мутацию - переставляем случайные пары местами
                RemoveLesson(pair1);//удаляем
                RemoveLesson(pair2);//удаляем
                var res1 = AddToAnyHour((int)pair2.pairInfo.Day, pair1);//вставляем в случайное место
                var res2 = AddToAnyHour((int)pair1.pairInfo.Day, pair2);//вставляем в случайное место
                return res1 && res2;
                //}
                //return false;
            }

            public IEnumerable<Lesson> GetLessonsOfDay(int day)
            {
                var count = day == 5 ? SaturdayHoursPerDay : HoursPerDay;

                for (int hour = 0; hour < count; hour++)
                    foreach (var p in HourPlans[day, hour].GroupInform)
                        yield return new Lesson(new PairInfo(hour, (DayOfWeek)day), p.Value.dropInfo, p.Value.dropInfoTwo);
            }

            public IEnumerable<Lesson> GetLessons()
            {
                for (int day = 0; day < DaysPerWeek; day++)
                    foreach (var l in GetLessonsOfDay(day))
                        yield return l;
            }

            public override string ToString()
            {
                var sb = new StringBuilder();
                //    for (int day = 0; day < Plan.DaysPerWeek; day++)
                //    {
                //        var count = day == 5 ? SaturdayHoursPerDay : HoursPerDay;

                //        sb.AppendFormat("Day {0}\r\n", day);
                //        for (int hour = 0; hour < count; hour++)
                //        {
                //            sb.AppendFormat("Hour {0}: ", hour);
                //            foreach (var p in HourPlans[day, hour].GroupInform)
                //                sb.AppendFormat(" УРОК: ({0}, {1}) {2} {3} {4} {5}\t", p.Value.Teacher, p.Value.Teacher.Department.NameOfDepartment, p.Value.Subject, p.Value.Group.Single().NameOfGroup, p.Value.Specifics, p.Value.NumberOfClassroom);
                //            sb.AppendLine();
                //        }

                //    }
                sb.AppendFormat("Fitness: {0}\r\n", FitnessValue);
                return sb.ToString();
            }

    }

        //План на час
        public class HourPlan
        {
            public Dictionary<int, Lesson> GroupInform = new Dictionary<int, Lesson>();
            public Dictionary<int, Lesson> TeacherInform = new Dictionary<int, Lesson>();
            public Dictionary<int, Lesson> ClassroomInform = new Dictionary<int, Lesson>();

            public Dictionary<int, Lesson> GroupInformTwo = new Dictionary<int, Lesson>();
            public Dictionary<int, Lesson> TeacherInformTwo = new Dictionary<int, Lesson>();
            public Dictionary<int, Lesson> ClassroomInformTwo = new Dictionary<int, Lesson>();

            public List<Lesson> Lessons { get; set; } = new List<Lesson>();

            public bool AddLesson(Lesson lesson)
            {
                if (lesson.dropInfo == null && lesson.dropInfoTwo == null)
                    return false;
                if (lesson.dropInfo != null)
                {
                    var groups = lesson.dropInfo.Group.Select(c => c.CodeOfGroup);
                    int classroom = lesson.dropInfo.NumberOfClassroom.CodeOfClassroom;
                    int teacher = lesson.dropInfo.Teacher.CodeOfTeacher;
                    if (groups.Any(g => GroupInform.ContainsKey(g)) || ClassroomInform.ContainsKey(classroom) || TeacherInform.ContainsKey(teacher))
                        return false;//в этот час уже есть пара у группы или в аудитории или у препода
                    if (lesson.dropInfo.Ndindex == 0)
                        if (groups.Any(g => GroupInformTwo.ContainsKey(g)) || ClassroomInformTwo.ContainsKey(classroom) || TeacherInformTwo.ContainsKey(teacher))
                            return false;//в этот час уже есть пара у группы или в аудитории или у препода
                }
                if (lesson.dropInfoTwo != null)
                {
                    var groups = lesson.dropInfoTwo.Group.Select(c => c.CodeOfGroup);
                    int classroom = lesson.dropInfoTwo.NumberOfClassroom.CodeOfClassroom;
                    int teacher = lesson.dropInfoTwo.Teacher.CodeOfTeacher;
                    var fgi =
                        new HashSet<int>(
                            GroupInform
                            .Where(gr => ((gr.Value.dropInfo?.Ndindex) ?? -1) == 0)
                            .Select(gr => gr.Key));
                    var fci =
                        new HashSet<int>(
                            ClassroomInform
                            .Where(gr => ((gr.Value.dropInfo?.Ndindex) ?? -1) == 0)
                            .Select(gr => gr.Key));
                    var fti =
                        new HashSet<int>(
                            TeacherInform
                            .Where(gr => ((gr.Value.dropInfo?.Ndindex) ?? -1) == 0)
                            .Select(gr => gr.Key));
                    if (groups.Any(g => fgi.Contains(g)) || fci.Contains(classroom) || fti.Contains(teacher))
                        return false;//в этот час уже есть пара у группы или в аудитории или у препода
                    if (groups.Any(g => GroupInformTwo.ContainsKey(g)) || ClassroomInformTwo.ContainsKey(classroom) || TeacherInformTwo.ContainsKey(teacher))
                        return false;//в этот час уже есть пара у группы или в аудитории или у препода
                }

                if (lesson.dropInfo != null)
                {
                    var groups = lesson.dropInfo.Group.Select(c => c.CodeOfGroup);
                    int classroom = lesson.dropInfo.NumberOfClassroom.CodeOfClassroom;
                    int teacher = lesson.dropInfo.Teacher.CodeOfTeacher;
                    foreach (var group in groups)
                        GroupInform[group] = lesson;

                    ClassroomInform[classroom] = lesson;
                    TeacherInform[teacher] = lesson;
                }
                if (lesson.dropInfoTwo != null)
                {
                    var groups2 = lesson.dropInfoTwo.Group.Select(c => c.CodeOfGroup);
                    int classroom2 = lesson.dropInfoTwo.NumberOfClassroom.CodeOfClassroom;
                    int teacher2 = lesson.dropInfoTwo.Teacher.CodeOfTeacher;

                    foreach (var group in groups2)
                        GroupInformTwo[group] = lesson;

                    ClassroomInformTwo[classroom2] = lesson;
                    TeacherInformTwo[teacher2] = lesson;
                }
                Lessons.Add(lesson);
                return true;
            }

            public void RemoveLesson(Lesson les)
            {
                Lesson temp = null;

                if (les.dropInfo != null)
                {
                    var groups = les.dropInfo.Group.Select(c => c.CodeOfGroup);
                    int classroom = les.dropInfo.NumberOfClassroom.CodeOfClassroom;
                    int teacher = les.dropInfo.Teacher.CodeOfTeacher;

                    ClassroomInform.TryGetValue(classroom, out temp);

                    foreach (var group in groups)
                        GroupInform.Remove(group);

                    ClassroomInform.Remove(classroom);
                    TeacherInform.Remove(teacher);
                }
                if (les.dropInfoTwo != null)
                {
                    var groups = les.dropInfoTwo.Group.Select(c => c.CodeOfGroup);
                    int classroom = les.dropInfoTwo.NumberOfClassroom.CodeOfClassroom;
                    int teacher = les.dropInfoTwo.Teacher.CodeOfTeacher;

                    ClassroomInformTwo.TryGetValue(classroom, out temp);

                    foreach (var group in groups)
                        GroupInformTwo.Remove(group);

                    ClassroomInformTwo.Remove(classroom);
                    TeacherInformTwo.Remove(teacher);
                }

                Lessons.RemoveAll(l => object.ReferenceEquals(l, temp));
            }

            public HourPlan Clone()
            {
                var res = new HourPlan();
                res.GroupInform = new Dictionary<int, Lesson>(GroupInform);
                res.ClassroomInform = new Dictionary<int, Lesson>(ClassroomInform);
                res.TeacherInform = new Dictionary<int, Lesson>(TeacherInform);
                res.GroupInformTwo = new Dictionary<int, Lesson>(GroupInformTwo);
                res.ClassroomInformTwo = new Dictionary<int, Lesson>(ClassroomInformTwo);
                res.TeacherInformTwo = new Dictionary<int, Lesson>(TeacherInformTwo);
                res.Lessons = new List<Lesson>(Lessons);
                return res;
            }
        }

        //пара
        public class Lesson
        {
            public PairInfo pairInfo;
            public DropInformation dropInfo;
            public DropInformation dropInfoTwo;

            public Lesson(DropInformation dropInformation)
            {
                if (dropInformation.Ndindex == -1)
                {
                    dropInfoTwo = dropInformation.Copy();
                }
                else
                {
                    dropInfo = dropInformation.Copy();
                }
            }

            public Lesson(PairInfo pi, DropInformation dropInfo, DropInformation dropInfoTwo)
            {
                pairInfo = new PairInfo(pi.Pair, pi.Day);
                this.dropInfo = dropInfo?.Copy();
                this.dropInfoTwo = dropInfoTwo?.Copy();
            }

            public Lesson(PairInfo pairinfo, DropInformation dropInformation) : this(dropInformation)
            {
                pairInfo = pairinfo;
            }
        }
    }
}