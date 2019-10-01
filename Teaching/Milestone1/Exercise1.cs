﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bmstu.IU6.Teaching
{
    public class Exercise1
    {
        /// <summary>
        /// База данных
        /// </summary>
        private Storage.DB db = new Storage.DB();

        /// <summary>
        /// Генератор случайных чисел
        /// </summary>
        private Random rnd = new Random();

        /// <summary>
        /// Генерация вектора неповторяющихся случайных чисел заданной длины
        /// </summary>
        /// <param name="count"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        private List<int> randoms(int count, int max)
        {
            List<int> list = new List<int>();
            for (int i = 1; i <= count; i++)
            {
                int n;
                do
                {
                    n = rnd.Next(1, max + 1);
                }
                while (list.Contains(n));
                list.Add(n);
            }

            return list;
        }

        /// <summary>
        /// Генерация уникальный заданий
        /// </summary>
        public void Run()
        {
            // Предварительная очистка списка
            db.Exercise1.RemoveRange(db.Exercise1.ToList());

            // Общее количество сотрудников
            int characterCount = db.Characters.Count();

            // Число ролей
            int roleCount = db.Roles.Count();

            // Формирование задания для каждого студента
            foreach (var student in db.Students.OrderBy(a => a.Family).ToList())
            {
                Console.WriteLine(student.Family);

                // Список сотрудников
                List<int> characters = randoms(roleCount, characterCount);

                // Формирование вариванта задания для каждой из шести ролей
                for (int n = 1; n <= roleCount; n++)
                {
                    int aCount = db.Abilities.Where(a => a.Role.Number == n).Count();
                    Storage.Exercise1 e1;
                    do
                    {
                        // Вектор способностей
                        List<int> abilities = randoms(2, aCount);
                        int a1 = abilities[0];
                        int a2 = abilities[1];

                        // Номер сотрудника
                        int cnumber = characters[n - 1];

                        e1 = new Storage.Exercise1()
                        {
                            Role = db.Roles.Where(a => a.Number == n).First(),
                            Character = db.Characters.Where(a => a.Number == cnumber).First(),
                            Ability1 = db.Abilities.Where(a => a.Role.Number == n && a.Number == a1).First(),
                            Ability2 = db.Abilities.Where(a => a.Role.Number == n && a.Number == a2).First(),
                            Student = student
                        };
                        e1.Code = e1.GetCode();
                    }
                    while (db.Exercise1.Where(a => a.Code == e1.Code).FirstOrDefault() != null);
                    db.Exercise1.Add(e1);
                }
            }
            db.SaveChanges();

            // Формирование выходного CSV-файла
            using (var wrt = new System.IO.StreamWriter(@"iu6.txt", false))
            {
                string s;
                s = "Студент;Группа;Код;";
                for (int i = 1; i <= 6; i++)
                {
                    s += $"И{i};Т{i};К{i}1;К{i}2;";
                }
                wrt.WriteLine(s);

                // Формирование задания для каждого студента
                foreach (var student in db.Students.OrderBy(a => a.Family).ToList())
                {
                    s = $"{student.FullName};{student.Group};{student.Code};";
                    var list = db.Exercise1.Where(a => a.Student.ID == student.ID).OrderBy(a => a.Character.Name).ToList();
                    foreach (Storage.Exercise1 e in list)
                    {
                        s += $"{e.Character.Name};{e.Character.Number};{e.Ability1.Name};{e.Ability2.Name};";
                    }
                    wrt.WriteLine(s);
                }
            }
        }
    }
}