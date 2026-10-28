using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Lab2.Part1_Interfaces
{
    public interface IMovable
    {
        void Move(int x, int y);
    }

    public class Point : IMovable
    {
        public int X { get; private set; }
        public int Y { get; private set; }

        public Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public void Move(int x, int y)
        {
            X += x;
            Y += y;
        }

        public override string ToString()
        {
            return "(" + X + ", " + Y + ")";
        }
    }

    public interface IDrawable
    {
        void Draw();
    }

    public interface IShape
    {
        double GetArea();
        double GetPerimeter();
    }

    public class Circle : IDrawable, IShape
    {
        public double Radius { get; set; }

        public Circle(double radius)
        {
            Radius = radius;
        }

        public void Draw()
        {
            Console.WriteLine("Отрисовка круга с радиусом " + Radius);
        }

        public double GetArea()
        {
            return Math.PI * Radius * Radius;
        }

        public double GetPerimeter()
        {
            return 2 * Math.PI * Radius;
        }
    }

    public class Rectangle : IDrawable, IShape
    {
        public double Width { get; set; }
        public double Height { get; set; }

        public Rectangle(double width, double height)
        {
            Width = width;
            Height = height;
        }

        public void Draw()
        {
            Console.WriteLine("Отрисовка прямоугольника " + Width + "x" + Height);
        }

        public double GetArea()
        {
            return Width * Height;
        }

        public double GetPerimeter()
        {
            return 2 * (Width + Height);
        }
    }

    public interface I3DShape : IShape
    {
        double GetVolume();
    }

    public class Cube : I3DShape
    {
        public double Edge { get; set; }

        public Cube(double edge)
        {
            Edge = edge;
        }

        public double GetArea()
        {
            return 6 * Edge * Edge;
        }

        public double GetPerimeter()
        {
            return 12 * Edge;
        }

        public double GetVolume()
        {
            return Edge * Edge * Edge;
        }
    }

    public interface IDevice
    {
        void Print();
        void Scan();
        void Fax();
    }

    public class OldPrinter : IDevice
    {
        public void Print()
        {
            Console.WriteLine("Старый принтер печатает");
        }

        public void Scan()
        {
        }

        public void Fax()
        {
        }
    }

    public class OldScanner : IDevice
    {
        public void Print()
        {
        }

        public void Scan()
        {
            Console.WriteLine("Старый сканер сканирует");
        }

        public void Fax()
        {
        }
    }

    public interface IPrinter
    {
        void Print();
    }

    public interface IScanner
    {
        void Scan();
    }

    public interface IFax
    {
        void Fax();
    }

    public class Printer : IPrinter
    {
        public void Print()
        {
            Console.WriteLine("Принтер: печать документа");
        }
    }

    public class Scanner : IScanner
    {
        public void Scan()
        {
            Console.WriteLine("Сканер: сканирование документа");
        }
    }

    public class MultifunctionDevice : IPrinter, IScanner, IFax
    {
        public void Print()
        {
            Console.WriteLine("МФУ: печать листа");
        }

        public void Scan()
        {
            Console.WriteLine("МФУ: сканирование в файл");
        }

        public void Fax()
        {
            Console.WriteLine("МФУ: отправка факса");
        }
    }

    public interface IPayable
    {
        void Pay(decimal amount);
    }

    public class CreditCard : IPayable
    {
        public string CardNumber { get; set; }

        public CreditCard(string cardNumber)
        {
            CardNumber = cardNumber;
        }

        public void Pay(decimal amount)
        {
            Console.WriteLine("Оплата картой " + CardNumber + " на сумму " + amount + " руб.");
        }
    }

    public class Cash : IPayable
    {
        public void Pay(decimal amount)
        {
            Console.WriteLine("Оплата наличными на сумму " + amount + " руб.");
        }
    }

    public interface ILogger
    {
        void Log(string message);
    }

    public class ConsoleLogger : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine("[ConsoleLogger]: " + message);
        }
    }

    public class FileLogger : ILogger
    {
        private readonly string filePath;

        public FileLogger(string filePath)
        {
            this.filePath = filePath;
        }

        public void Log(string message)
        {
            File.AppendAllText(filePath, "[FileLogger " + DateTime.Now.ToString("HH:mm:ss") + "]: " + message + Environment.NewLine);
            Console.WriteLine("[FileLogger]: Записано в файл: " + message);
        }
    }

    internal class Program
    {
        static void DrawAll(List<IDrawable> items)
        {
            foreach (IDrawable item in items)
            {
                item.Draw();
            }
        }

        static void PrintShapeInfo(IShape shape)
        {
            Console.WriteLine("Площадь: " + shape.GetArea().ToString("F2") + ", Периметр: " + shape.GetPerimeter().ToString("F2"));
        }

        static void ProcessPayment(IPayable method, decimal amount)
        {
            method.Pay(amount);
        }

        static void DoWork(ILogger logger)
        {
            logger.Log("Старт бизнес-процесса");
            logger.Log("Выполнение операций...");
            logger.Log("Завершение процесса");
        }

        static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Console.WriteLine("================ ЧАСТЬ 1. БАЗОВЫЕ ИНТЕРФЕЙСЫ ================\n");

            Console.WriteLine("--- Задание 1. Базовые интерфейсы ---");
            Point point = new Point(10, 20);
            Console.WriteLine("Начальная точка: " + point);
            point.Move(5, -3);
            Console.WriteLine("После Move(5, -3): " + point);

            List<IDrawable> figures = new List<IDrawable>
            {
                new Circle(5),
                new Rectangle(8, 4)
            };
            DrawAll(figures);
            Console.WriteLine();

            Console.WriteLine("--- Задание 2. Интерфейсы и наследование ---");
            IShape c = new Circle(6);
            Console.Write("Круг R=6: ");
            PrintShapeInfo(c);

            IShape r = new Rectangle(5, 10);
            Console.Write("Прямоугольник 5x10: ");
            PrintShapeInfo(r);

            Cube cube = new Cube(3);
            Console.WriteLine("Куб с ребром 3:");
            Console.WriteLine("Площадь поверхности: " + cube.GetArea());
            Console.WriteLine("Периметр ребер: " + cube.GetPerimeter());
            Console.WriteLine("Объем: " + cube.GetVolume());
            Console.WriteLine();

            Console.WriteLine("--- Задание 3. Сегрегация интерфейсов (ISP) ---");
            Printer p = new Printer();
            p.Print();

            Scanner s = new Scanner();
            s.Scan();

            MultifunctionDevice mfd = new MultifunctionDevice();
            mfd.Print();
            mfd.Scan();
            mfd.Fax();
            Console.WriteLine();

            Console.WriteLine("--- Задание 4. Интерфейсы и полиморфизм ---");
            IPayable card = new CreditCard("****-4455");
            IPayable cash = new Cash();
            ProcessPayment(card, 12000);
            ProcessPayment(cash, 2500);

            ILogger cLog = new ConsoleLogger();
            ILogger fLog = new FileLogger("part1_log.txt");
            DoWork(cLog);
            DoWork(fLog);

            if (File.Exists("part1_log.txt"))
            {
                File.Delete("part1_log.txt");
            }
        }
    }
}
