using System.Xml.Linq;
using Raspisanie.Models;
using System.Collections.Generic;
using System.Linq;
using FirebirdSql.Data.FirebirdClient;
using System;
using System.Windows;
using Raspisanie.ViewModels;
using System.Collections.ObjectModel;
using System.Collections;

namespace Raspisanie
{
    class XML
    {
        public static IEnumerable<Teacher> ReadTeacher(string path)
        {
            if (System.IO.File.Exists(path))
            {
                var xd = XDocument.Load(path);
                foreach (var teacher in xd.Root.Elements("Teacher"))
                {
                    var CodeOfTeacher = (int)teacher.Element("CodeOfTeacher");
                    var FIO = (string)teacher.Element("FIO");
                    var Post = (string)teacher.Element("Post");
                    var HourOfLoad = (int)teacher.Element("HourOfLoad");
                    var CodeOfDepartment = (int)teacher.Element("CodeOfDepartment");
                    yield return
                        new Teacher
                        {
                            CodeOfTeacher = CodeOfTeacher,
                            FIO = FIO,
                            Post = Post,
                        };
                }
            }
        }

        public static IEnumerable<Subject> ReadSubject(string path)
        {
            if (System.IO.File.Exists(path))
            {
                var xd = XDocument.Load(path);
                foreach (var subject in xd.Root.Elements("Subject"))
                {
                    var NameOfSubject = (string)subject.Element("NameOfSubject");
                    var Specifics = (string)subject.Element("Specifics");
                    var LectureHours = (int)subject.Element("LectureHours");
                    var ExerciseHours = (int)subject.Element("ExerciseHours");
                    var LaboratoryHours = (int)subject.Element("LaboratoryHours");
                    var CommonHours = (int)subject.Element("CommonHours");
                    var CodeOfDepartment = (int)subject.Element("CodeOfDepartment");

                    yield return
                        new Subject
                        {
                            NameOfSubject = NameOfSubject,
                         //   Specifics =Specifics,
                            CodeOfDepartment = CodeOfDepartment
                        };
                }
            }
        }

        public static IEnumerable<Group> ReadGroup(string path)
        {
            if (System.IO.File.Exists(path))
            {
                var xd = XDocument.Load(path);
                foreach (var group in xd.Root.Elements("Group"))
                {
                    var NameOfGroup = (string)group.Element("NameOfGroup");
                    var CodeOfGroup = (int)group.Element("CodeOfGroup");
                    var CodeOfDepartment = (int)group.Element("CodeOfDepartment");
                    yield return
                        new Group
                        {
                            NameOfGroup = NameOfGroup,
                            CodeOfGroup =CodeOfGroup,
                            CodeOfDepartment = CodeOfDepartment

                        };
                }
            }
        }

       
    }
}
