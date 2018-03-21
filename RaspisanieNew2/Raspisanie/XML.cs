using System.Xml.Linq;
using Raspisanie.Models;
using System.Collections.Generic;
using System.Linq;

namespace Raspisanie
{
    class XML
    {
        public static XElement CreateXMLFaculty(Faculty faculty)
        {
            return new XElement("Faculty",
                new XElement("CodeOfFaculty", faculty.CodeOfFaculty),
                new XElement("NameOfFaculty", faculty.NameOfFaculty));
        }

        public static XElement CreateXMLClassroom(ClassRoom classroom)
        {
            return new XElement("Classroom",
                new XElement("NumberOfClassroom", classroom.NumberOfClassroom),
                new XElement("Specifics", classroom.Specifics),
                new XElement("CodeOfClassroom", classroom.CodeOfClassroom),
                new XElement("CodeOfDepartment", classroom.CodeOfDepartment));
        }

        public static XElement CreateXMLDepartment(Department department)
        {
            return new XElement("Department",
                new XElement("NameOfDepartment", department.NameOfDepartment),
                new XElement("CodeOfDepartment", department.CodeOfDepartment),
                new XElement("CodeOfFaculty", department.CodeOfFaculty));
        }

        public static XElement CreateXMLTeacher(Teacher teacher)
        {
            return new XElement("Teacher",
                new XElement("CodeOfTeacher", teacher.CodeOfTeacher),
                new XElement("FIO",teacher.FIO),
                new XElement("Post", teacher.Post));
        }

        public static XElement CreateXMLSubject(Subject subject)
        {
            return new XElement("Subject",
                new XElement("NameOfSubject", subject.NameOfSubject),
                new XElement("Specifics", subject.Specifics),
                new XElement("CodeOfDepartment", subject.CodeOfDepartment));
        }

        public static XElement CreateXMLGroup(Group group)
        {
            return new XElement("Group",
                new XElement("NameOfGroup", group.NameOfGroup),
                new XElement("CodeOfGroup", group.CodeOfGroup),
                new XElement("CodeOfDepartment", group.CodeOfDepartment));
        }

        public static IEnumerable<Faculty> ReadFaculty(string path)
        {
            if (System.IO.File.Exists(path))
            {
                var xd = XDocument.Load(path);
                foreach (var faculty in xd.Root.Elements("Faculty"))
                {
                    var CodeOfFaculty = (int)faculty.Element("CodeOfFaculty");
                    var NameOfFaculty = (string)faculty.Element("NameOfFaculty");

                    yield return
                        new Faculty
                        {
                            CodeOfFaculty = CodeOfFaculty,
                            NameOfFaculty = NameOfFaculty
                        };
                }

            }

        }

        public static IEnumerable<ClassRoom> ReadClassroom(string path)
        {
            if (System.IO.File.Exists(path))
            {
                var xd = XDocument.Load(path);
                foreach (var classroom in xd.Root.Elements("Classroom"))
                {
                    var NumberOfClassroom = (string)classroom.Element("NumberOfClassroom");
                    var Capacity = (int)classroom.Element("Capacity");
                    var Specifics = (string)classroom.Element("Specifics");
                    var CodeOfClassroom = (int)classroom.Element("CodeOfClassroom");
                    var CodeOfDepartment = (int)classroom.Element("CodeOfDepartment");
                    yield return
                            new ClassRoom
                            {
                                NumberOfClassroom = NumberOfClassroom,
                             
                                Specifics = Specifics,
                                CodeOfClassroom =CodeOfClassroom,
                                CodeOfDepartment = CodeOfDepartment
                            };
                }

            }
        }

        public static IEnumerable<Department> ReadDepartment(string path)
        {
            if (System.IO.File.Exists(path))
            {
                var xd = XDocument.Load(path);
                foreach (var department in xd.Root.Elements("Department"))
                {
                    var CodeOfFaculty = (int)department.Element("CodeOfFaculty");
                    var CodeOfDepartment = (int)department.Element("CodeOfDepartment");
                    var NameOfDepartment = (string)department.Element("NameOfDepartment");
                    yield return
                            new Department
                            {
                                CodeOfFaculty = CodeOfFaculty,
                                CodeOfDepartment = CodeOfDepartment,
                                NameOfDepartment = NameOfDepartment
                            };
                }

            }
        }

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
                            Specifics =Specifics,
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

        public static void SaveClassroom(IEnumerable<ClassRoom> collection, string path)
        {
            if (collection.Any())
            {
                var xmlclassroom = new XDocument(new XElement("root"));

                foreach (var classroom in collection)
                {
                    var elclassroom = CreateXMLClassroom(classroom);
                    xmlclassroom.Root.Add(elclassroom);
                }

                var dir = System.IO.Path.GetDirectoryName(path);
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);

                xmlclassroom.Save(path);
            }
        }

        public static void SaveFaculty(IEnumerable<Faculty> collection, string path)
        {
            if (collection.Any())
            {
                var xmlfaculty = new XDocument(new XElement("root"));
                
                foreach (var faculty in collection)
                {
                    var elfaculty = CreateXMLFaculty(faculty);
                    xmlfaculty.Root.Add(elfaculty);
                }

                var dir = System.IO.Path.GetDirectoryName(path);
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);

                xmlfaculty.Save(path);
            }
        }

        public static void SaveDepartment(IEnumerable<Department> collection, string path)
        {
            if (collection.Any())
            {
                var xmldepartment = new XDocument(new XElement("root"));

                foreach (var department in collection)
                {
                    var eldepartment = CreateXMLDepartment(department);
                    xmldepartment.Root.Add(eldepartment);
                }

                var dir = System.IO.Path.GetDirectoryName(path);
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);

                xmldepartment.Save(path);
            }
        }

        public static void SaveTeacher(IEnumerable<Teacher> collection, string path)
        {
            if (collection.Any())
            {
                var xmlteacher = new XDocument(new XElement("root"));
                foreach (var teacher in collection)
                {
                    var elteacher = CreateXMLTeacher(teacher);
                    xmlteacher.Root.Add(elteacher);
                }

                var dir = System.IO.Path.GetDirectoryName(path);
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);

                xmlteacher.Save(path);
            }
        }

        public static void SaveSubject(IEnumerable<Subject> collection, string path)
        {
            if (collection.Any())
            {
                var xmlsubject = new XDocument(new XElement("root"));
                foreach (var subject in collection)
                {
                    var elsubject = CreateXMLSubject(subject);
                    xmlsubject.Root.Add(elsubject);
                }
                var dir = System.IO.Path.GetDirectoryName(path);
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);

                xmlsubject.Save(path);
            }
        }

        public static void SaveGroup(IEnumerable<Group> collection, string path)
        {
            if(collection.Any())
            {
                var xmlgroup = new XDocument(new XElement("root"));

                foreach (var group in collection)
                {
                    var elgroup = CreateXMLGroup(group);
                    xmlgroup.Root.Add(elgroup);
                }

                var dir = System.IO.Path.GetDirectoryName(path);
                if (!System.IO.Directory.Exists(dir))
                    System.IO.Directory.CreateDirectory(dir);

                xmlgroup.Save(path);
            }
        }
    }
}
