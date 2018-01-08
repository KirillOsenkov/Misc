using System;
using System.Collections.Generic;
using System.Linq;

struct Point3D
{
    public readonly int X;
    public readonly int Y;
    public readonly int Z;

    public Point3D(int x, int y, int z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public static Point3D operator +(Point3D left, Point3D right)
    {
        return new Point3D(left.X + right.X, left.Y + right.Y, left.Z + right.Z);
    }

    public static IEnumerable<Point3D> EnumerateAllPoints(int x, int y, int z)
    {
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                for (int k = 0; k < z; k++)
                {
                    yield return new Point3D(i, j, k);
                }
            }
        }
    }

    public override string ToString()
    {
        return $"({X},{Y},{Z})";
    }
}

class Shape3D : IEquatable<Shape3D>
{
    private bool[,,] array;
    public int SizeX;
    public int SizeY;
    public int SizeZ;

    public Shape3D(int sizeX, int sizeY, int sizeZ)
    {
        array = new bool[sizeX, sizeY, sizeZ];
        this.SizeX = sizeX;
        this.SizeY = sizeY;
        this.SizeZ = sizeZ;
    }

    public Shape3D(bool[,,] array)
    {
        this.array = array;
        this.SizeX = array.GetLength(0);
        this.SizeY = array.GetLength(1);
        this.SizeZ = array.GetLength(2);
    }

    public Shape3D(IEnumerable<Point3D> points)
        : this(points.ToArray())
    {
    }

    public Shape3D Clone()
    {
        return new Shape3D(GetPoints());
    }

    public Shape3D(params Point3D[] points)
    {
        int offsetX = points.Min(p => p.X);
        int offsetY = points.Min(p => p.Y);
        int offsetZ = points.Min(p => p.Z);
        this.SizeX = points.Max(p => p.X) + 1 - offsetX;
        this.SizeY = points.Max(p => p.Y) + 1 - offsetY;
        this.SizeZ = points.Max(p => p.Z) + 1 - offsetZ;
        array = new bool[SizeX, SizeY, SizeZ];
        foreach (var p in points)
        {
            array[p.X - offsetX, p.Y - offsetY, p.Z - offsetZ] = true;
        }
    }

    public override string ToString()
    {
        return string.Join(",", GetPoints());
    }

    public bool this[int x, int y, int z]
    {
        get
        {
            return array[x, y, z];
        }

        set
        {
            array[x, y, z] = value;
        }
    }

    public bool this[Point3D point]
    {
        get
        {
            return this[point.X, point.Y, point.Z];
        }

        set
        {
            this[point.X, point.Y, point.Z] = value;
        }
    }

    public bool IsPointFree(Point3D point)
    {
        if (point.X >= 0 &&
            point.X < SizeX &&
            point.Y >= 0 &&
            point.Y < SizeY &&
            point.Z >= 0 &&
            point.Z < SizeZ &&
            !array[point.X, point.Y, point.Z])
        {
            return true;
        }

        return false;
    }

    public IEnumerable<Point3D> GetPoints()
    {
        for (int x = 0; x < SizeX; x++)
        {
            for (int y = 0; y < SizeY; y++)
            {
                for (int z = 0; z < SizeZ; z++)
                {
                    if (array[x, y, z])
                    {
                        yield return new Point3D(x, y, z);
                    }
                }
            }
        }
    }

    public Shape3D[] GetAllRotations()
    {
        var result = new HashSet<Shape3D>(EqualityComparer<Shape3D>.Default);

        var xshape = this.Clone();
        for (int x = 0; x <= 3; x++)
        {
            var yshape = xshape.Clone();
            for (int y = 0; y <= 3; y++)
            {
                var zshape = yshape.Clone();
                for (int z = 0; z <= 3; z++)
                {
                    result.Add(zshape);
                    zshape = zshape.RotateZ();
                }

                yshape = yshape.RotateY();
            }

            xshape = xshape.RotateX();
        }

        return result.ToArray();
    }

    public Shape3D RotateX()
    {
        return new Shape3D(GetPoints().Select(p => new Point3D(p.X, SizeZ - p.Z, p.Y)));
    }

    public Shape3D RotateY()
    {
        return new Shape3D(GetPoints().Select(p => new Point3D(SizeZ - p.Z, p.Y, p.X)));
    }

    public Shape3D RotateZ()
    {
        return new Shape3D(GetPoints().Select(p => new Point3D(SizeY - p.Y, p.X, p.Z)));
    }

    public bool CanAddShape(Shape3D shape, Point3D point)
    {
        foreach (var p in shape.GetPoints())
        {
            if (!IsPointFree(p + point))
            {
                return false;
            }
        }

        return true;
    }

    public bool Equals(Shape3D other)
    {
        if (other == null)
        {
            return false;
        }

        if (SizeX != other.SizeX || SizeY != other.SizeY || SizeZ != other.SizeZ)
        {
            return false;
        }

        for (int i = 0; i < SizeX; i++)
        {
            for (int j = 0; j < SizeY; j++)
            {
                for (int k = 0; k < SizeZ; k++)
                {
                    if (this[i, j, k] != other[i, j, k])
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    public override int GetHashCode()
    {
        int num = 0;
        for (int i = 0; i < SizeX; i++)
        {
            for (int j = 0; j < SizeY; j++)
            {
                for (int k = 0; k < SizeZ; k++)
                {
                    if (this[i, j, k])
                    {
                        int bit = (i << 16) + (j << 8) + k;
                        num |= bit;
                    }
                }
            }
        }

        return num;
    }

    public void AddShape(Shape3D shape3D, Point3D currentPosition)
    {
        foreach (var p in shape3D.GetPoints())
        {
            this[p + currentPosition] = true;
        }
    }

    public void RemoveShape(Shape3D shape3D, Point3D currentPosition)
    {
        foreach (var p in shape3D.GetPoints())
        {
            this[p + currentPosition] = false;
        }
    }
}

class Program
{
    private static Shape3D container;
    private static int[] currentRotations;
    private static int pieceCount;
    private static Shape3D[] pieces;
    private static List<Shape3D[]> shapeRotations;

    static void Main(string[] args)
    {
        container = new Shape3D(5, 4, 2);
        pieces = new Shape3D[]
        {
            new Shape3D(
                P(0,0,0),
                P(1,0,0),
                P(2,0,0),
                P(0,1,0)),
            new Shape3D(
                P(0,0,0),
                P(1,0,0),
                P(0,1,0),
                P(0,0,1)),
            new Shape3D(
                P(0,0,0),
                P(1,0,0),
                P(0,1,0),
                P(1,1,0)),
            new Shape3D(
                P(0,0,0),
                P(0,0,1),
                P(0,1,0),
                P(1,1,0)),
            new Shape3D(
                P(0,0,0),
                P(1,0,0)),
            new Shape3D(
                P(0,0,0),
                P(1,0,0),
                P(1,1,0),
                P(2,0,0)),
            new Shape3D(
                P(0,0,0),
                P(1,0,0),
                P(2,0,0),
                P(3,0,0)),
            new Shape3D(
                P(0,0,0),
                P(1,0,0),
                P(2,0,0)),
            new Shape3D(
                P(0,0,0),
                P(1,0,0),
                P(0,1,0)),
            new Shape3D(
                P(0,0,0),
                P(1,0,0),
                P(1,1,0),
                P(2,1,0)),
            new Shape3D(
                P(0,0,0),
                P(0,0,1),
                P(1,0,0),
                P(1,1,0))
        };

        pieceCount = pieces.Length;

        shapeRotations = new List<Shape3D[]>();
        foreach (var piece in pieces)
        {
            shapeRotations.Add(piece.GetAllRotations().Where(r =>
                r.SizeX <= container.SizeX &&
                r.SizeY <= container.SizeY &&
                r.SizeZ <= container.SizeZ).ToArray());
        }

        currentRotations = new int[pieceCount];
        int count = 0;
        int shape = 0;
        int carry = 0;
        while (true)
        {
            TryPlacement();

            if (currentRotations[shape] < shapeRotations[shape].Length - 1)
            {
                count++;
                currentRotations[shape]++;
                shape = shape - carry;
                carry = 0;
            }
            else
            {
                currentRotations[shape] = 0;
                shape++;
                carry++;
                if (shape >= pieceCount)
                {
                    break;
                }
            }
        }
    }

    private static void TryPlacement()
    {
        List<Shape3D> shapes = new List<Shape3D>();
        for (int i = 0; i < pieceCount; i++)
        {
            var rotation = shapeRotations[i][currentRotations[i]];
            shapes.Add(rotation);
        }

        Point3D[] positions = new Point3D[shapes.Count];

        int shape = 0;
        while (true)
        {
            var currentPosition = positions[shape];

            if (container.CanAddShape(shapes[shape], currentPosition))
            {
                container.AddShape(shapes[shape], currentPosition);
                shape++;
                if (shape == shapes.Count)
                {
                    throw null;
                }

                continue;
            }

            increment:
            if (currentPosition.X < container.SizeX - shapes[shape].SizeX + 1 &&
                currentPosition.Y < container.SizeY - shapes[shape].SizeY + 1 &&
                currentPosition.Z < container.SizeZ - shapes[shape].SizeZ + 1)
            {
                positions[shape] = new Point3D(currentPosition.X + 1, currentPosition.Y, currentPosition.Z);
            }
            else if (currentPosition.Y < container.SizeY - shapes[shape].SizeY + 1 &&
                     currentPosition.Z < container.SizeZ - shapes[shape].SizeZ + 1)
            {
                positions[shape] = new Point3D(0, currentPosition.Y + 1, currentPosition.Z);
            }
            else if (currentPosition.Z < container.SizeZ - shapes[shape].SizeZ + 1)
            {
                positions[shape] = new Point3D(0, 0, currentPosition.Z + 1);
            }
            else
            {
                positions[shape] = new Point3D();
                shape--;
                if (shape < 0)
                {
                    return;
                }

                container.RemoveShape(shapes[shape], positions[shape]);
                currentPosition = positions[shape];
                goto increment;
            }
        }
    }

    static Point3D P(int x, int y, int z)
    {
        return new Point3D(x, y, z);
    }
}