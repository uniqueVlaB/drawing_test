using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Numerics;

namespace drawing_test
{
    class Program
    {
        static void Main(string[] args)
        {
            // Створюємо список планет
            Map.planets = new List<Planet>();
            // Встановлюємо кількість планет
            Map.numPlanets = 50;
            // Встановлюємо розмірність карти
            Map.height = 600;
            Map.width = 800;
            // Встановлюємо мінімальну відстань між планетами
            Map.minDistanceBetweenPlanets = 50;
            // Встановлюємо мінімальну відстань від планети до лінії з'єднання
            Map.minDistanceFromPlanetToEdge = 25;

            // Генеруємо планети випадковим чином
            Random rnd = new Random();
            for (int i = 0; i < Map.numPlanets; i++)
            {
                PointF position = new PointF(rnd.Next(Map.width), rnd.Next(Map.height));
                while (Map.planets.Any(p => Distance(p.Position, position) < Map.minDistanceBetweenPlanets))
                {
                    position = new PointF(rnd.Next(Map.width), rnd.Next(Map.height));
                }
                Planet planet = new Planet(position);
                Map.planets.Add(planet);
            }

            // Створюємо список з'єднань між планетами
            Map.edges = new List<Edge>();

            // З'єднуємо кожну планету із найближчим сусідом
            foreach (Planet planet in Map.planets)
            {
                List<Planet> neighbors = Map.planets
                    .OrderBy(p => Distance(p.Position, planet.Position))
                    .Where(p => p != planet)
                    .Take(3)
                    .ToList();

                foreach (Planet neighbor in neighbors)
                {

                    Edge connection = new Edge(planet, neighbor);
                    // Перевірте, чи перетинається нове з'єднання з існуючими
                    bool intersects = false;
                    foreach (Edge existingConnection in Map.edges)
                    {
                        if (existingConnection.from == connection.from || existingConnection.from == connection.to ||
                            existingConnection.to == connection.from || existingConnection.to == connection.to)
                        {
                            // Якщо з'єднання має спільний початок або кінець, це не перетин
                            continue;
                        }

                        if (Intersects(connection.from.Position, connection.to.Position, existingConnection.from.Position, existingConnection.to.Position))
                        {
                            intersects = true;
                            break;
                        }
                    }

                    if (!intersects)
                    {
                        // Перевірка, що з'єднання не проходить занадто близько до інших планет
                        bool tooClose = false;
                        foreach (Planet otherPlanet in Map.planets.Where(p => p != planet && p != neighbor))
                        {
                            if (DistanceToSegment(connection.from.Position, connection.to.Position, otherPlanet.Position) < Map.minDistanceFromPlanetToEdge)
                            {
                                tooClose = true;
                                break;
                            }
                        }
                        if (!tooClose)
                        {
                            Map.edges.Add(connection);
                            planet.Connections.Add(neighbor);
                            neighbor.Connections.Add(planet);
                        }
                    }
                }
            }


            Bitmap bmp = new Bitmap(Map.width, Map.height);

            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(Color.White);
            }

            using (Graphics g = Graphics.FromImage(bmp))
            {
                foreach (Planet planet in Map.planets)
                {
                    g.FillEllipse(Brushes.Red, planet.Position.X - 5, planet.Position.Y - 5, 10, 10);
                }
            }

            using (Graphics g = Graphics.FromImage(bmp))
            {
                foreach (Edge connection in Map.edges)
                {
                    g.DrawLine(Pens.Black, connection.from.Position, connection.to.Position);
                }
            }

            bmp.Save("planets.bmp");
        }

        static float DistanceToSegment(PointF a, PointF b, PointF p)
        {
            float l2 = Distance(a, b) * Distance(a, b);
            if (l2 == 0)
            {
                return Distance(p, a);
            }

            float t = ((p.X - a.X) * (b.X - a.X) + (p.Y - a.Y) * (b.Y - a.Y)) / l2;
            if (t < 0)
            {
                return Distance(p, a);
            }
            else if (t > 1)
            {
                return Distance(p, b);
            }

            PointF projection = new PointF(a.X + t * (b.X - a.X), a.Y + t * (b.Y - a.Y));
            return Distance(p, projection);
        }

        static float Distance(PointF p1, PointF p2)
        {
            float dx = p1.X - p2.X;
            float dy = p1.Y - p2.Y;
            return (float)Math.Sqrt(dx * dx + dy * dy);
        }

        static bool Intersects(PointF a1, PointF a2, PointF b1, PointF b2)
        {
            float d1 = CrossProduct(new PointF(b1.X - a1.X, b1.Y - a1.Y), new PointF(a2.X - a1.X, a2.Y - a1.Y));
            float d2 = CrossProduct(new PointF(b2.X - a1.X, b2.Y - a1.Y), new PointF(a2.X - a1.X, a2.Y - a1.Y));
            float d3 = CrossProduct(new PointF(a1.X - b1.X, a1.Y - b1.Y), new PointF(b2.X - b1.X, b2.Y - b1.Y));
            float d4 = CrossProduct(new PointF(a2.X - b1.X, a2.Y - b1.Y), new PointF(b2.X - b1.X, b2.Y - b1.Y));
            if (d1 * d2 < 0 && d3 * d4 < 0)
            {
                return true;
            }
            if (d1 == 0 && OnSegment(a1, a2, b1))
            {
                return true;
            }
            if (d2 == 0 && OnSegment(a1, a2, b2))
            {
                return true;
            }
            if (d3 == 0 && OnSegment(b1, b2, a1))
            {
                return true;
            }
            if (d4 == 0 && OnSegment(b1, b2, a2))
            {
                return true;
            }
            return false;
        }

        static float CrossProduct(PointF a, PointF b)
        {
            return a.X * b.Y - b.X * a.Y;
        }

        static bool OnSegment(PointF a, PointF b, PointF c)
        {
            if (c.X >= Math.Min(a.X, b.X) && c.X <= Math.Max(a.X, b.X) &&
                c.Y >= Math.Min(a.Y, b.Y) && c.Y <= Math.Max(a.Y, b.Y))
            {
                return true;
            }
            return false;
        }




    }
}