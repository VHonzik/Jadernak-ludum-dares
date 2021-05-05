using System;
using System.Collections.Generic;


    public struct Vector3Int
    {
        public int x;
        public int y;
        public int z;

        public Vector3Int(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        public Vector3Int(Vector3Int p1, Vector3Int p2)
        {
            this.x = p1.x + p2.x;
            this.y = p1.y + p2.y;
            this.z = p1.z + p2.z;
        }

        public int GetDistanceSquared(Vector3Int point)
        {
            int dx = this.x - point.x;
            int dy = this.y - point.y;
            int dz = this.z - point.z;
            return (dx * dx) + (dy * dy) + (dz * dz);
        }

        public bool EqualsSS(Vector3Int p)
        {
            return p.x == this.x && p.z == this.z && p.y == this.y;
        }

        public override int GetHashCode()
        {
            return (x + " " + y + " " + z).GetHashCode();
        }

        public override string ToString()
        {
            return x + ", " + y + ", " + z;
        }

        public static bool operator ==(Vector3Int one, Vector3Int two)
        {
            return one.EqualsSS(two);
        }

        public static bool operator !=(Vector3Int one, Vector3Int two)
        {
            return !one.EqualsSS(two);
        }
        
        public static bool operator >(Vector3Int one, Vector3Int two)
        {
            return ((one.x * one.x + one.y * one.y + one.z * one.z) > (two.x * two.x + two.y * two.y + two.z * two.z));
        }
        public static bool operator <(Vector3Int one, Vector3Int two)
        {
            return ((one.x * one.x + one.y * one.y + one.z * one.z) < (two.x * two.x + two.y * two.y + two.z * two.z));
        }
        
        /*
        public static bool operator >(Vector3Int one, Vector3Int two)
        {
            return ((one.x + one.y + one.z) > (two.x + two.y + two.z));
        }
        public static bool operator <(Vector3Int one, Vector3Int two)
        {
            return ((one.x + one.y + one.z) < (two.x + two.y + two.z));
        }
        */
        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public static Vector3Int operator +(Vector3Int one, Vector3Int two)
        {
            return new Vector3Int(one.x + two.x, one.y + two.y, one.z + two.z);
        }

        public static Vector3Int operator -(Vector3Int one, Vector3Int two)
        {
            return new Vector3Int(one.x - two.x, one.y - two.y, one.z - two.z);
        }

        public static Vector3Int zero = new Vector3Int(0, 0, 0);
    }

    public struct Vector2Int
    {
        public int x;
        public int y;

        public Vector2Int(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Vector2Int(Vector2Int p1, Vector2Int p2)
        {
            this.x = p1.x + p2.x;
            this.y = p1.y + p2.y;
        }

        public int GetDistanceSquared(Vector2Int point)
        {
            int dx = this.x - point.x;
            int dy = this.y - point.y;

            return (dx * dx) + (dy * dy);
        }

        public bool EqualsSS(Vector2Int p)
        {
            return p.x == this.x && p.y == this.y;
        }

        public override int GetHashCode()
        {
            return (x + " " + y).GetHashCode();
        }

        public override string ToString()
        {
            return x + ", " + y;
        }

        public static bool operator ==(Vector2Int one, Vector2Int two)
        {
            return one.EqualsSS(two);
        }

        public static bool operator !=(Vector2Int one, Vector2Int two)
        {
            return !one.EqualsSS(two);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public static Vector2Int operator +(Vector2Int one, Vector2Int two)
        {
            return new Vector2Int(one.x + two.x, one.y + two.y);
        }

        public static Vector2Int operator -(Vector2Int one, Vector2Int two)
        {
            return new Vector2Int(one.x - two.x, one.y - two.y);
        }

        public static Vector2Int zero = new Vector2Int(0, 0);
    }

