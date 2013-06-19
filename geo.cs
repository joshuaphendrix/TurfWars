using System;

public static class Geo
{
    public const double WGS84_a = 6378137.0;  // Major semiaxis [m]
    public const double WGS84_b = 6356752.3;  // Minor semiaxis [m]

    public static double WGS84Radius(double lat)
    {
        //http://en.wikipedia.org/wiki/Earth_radius
        double An = WGS84_a*WGS84_a * Math.Cos(lat);
        double Bn = WGS84_b*WGS84_b * Math.Sin(lat);
        double Ad = WGS84_a * Math.Cos(lat);
        double Bd = WGS84_b * Math.Sin(lat);
        return Math.Sqrt((An * An + Bn * Bn) / (Ad * Ad + Bd * Bd));
    }

    public static double dpsToDeg(Dps dps)
    {
        return dpsToDeg(dps.degrees, dps.primes, dps.seconds);
    }
    public static double dpsToDeg(double degrees, double primes, double seconds)
    {
        return degrees + primes / 60.0 + seconds / 3600.0;
    }

    public static Dps degToDps(double degrees)
    {
        double int_degrees = Math.Floor(degrees);
        double primes = (degrees - int_degrees) * 60.0;
        double int_primes = Math.Floor(primes);
        double seconds = (primes - int_primes) * 60.0;
        double int_seconds = Math.Round(seconds);

        Dps dps = new Dps();

        dps.degrees = (Int16)int_degrees;
        dps.primes = (Int16)int_primes;
        dps.seconds = (Int16)int_seconds;

        return dps;
    }

    private static double degreeToRadian(double angle)
    {
        return Math.PI * angle / 180.0;
    }
    private static double radianToDegree(double angle)
    {
        return angle * (180.0 / Math.PI);
    }

    public static Box getBoundingBox(double latDeg, double lonDeg, double km)
    {
        Box box = new Box();
        Double lat = degreeToRadian(latDeg);
        Double lon = degreeToRadian(lonDeg);
        km = 1000*km/2;

        //Radius of Earth at given latitude
        Double radius = WGS84Radius(lat);
        //Radius of the parallel at given latitude
        Double pradius = radius*Math.Cos(lat);

        Double latMin = lat - km/radius;
        Double latMax = lat + km/radius;
        Double lonMin = lon - km/pradius;
        Double lonMax = lon + km/pradius;

        //Response.Write(String.Format("{0},{1},{2},{3}", box.north, box.east, box.west,box.south));

        box.north = radianToDegree(latMax);
        box.south = radianToDegree(latMin);
        box.east = radianToDegree(lonMax);
        box.west = radianToDegree(lonMin);

        return box;
    }
    public static Box getBox(double lat, double lng, int seconds)
    {
        Box box = new Box();

        Geo.Dps dpsNorth = Geo.degToDps(lat);
        Geo.Dps dpswest = Geo.degToDps(lng);
        Geo.Dps dpssouth = Geo.degToDps(lat);
        Geo.Dps dpseast = Geo.degToDps(lng);

        int iN = dpsNorth.seconds;
        int iW = dpswest.seconds;


        dpsNorth.seconds = ((Int16)(Math.Round(dpsNorth.seconds / 10.0) * 10));
        dpswest.seconds = ((Int16)(Math.Round(dpswest.seconds / 10.0) * 10));
        dpssouth.seconds = ((Int16)(Math.Round(dpssouth.seconds / 10.0) * 10));
        dpseast.seconds = ((Int16)(Math.Round(dpseast.seconds / 10.0) * 10));


        if (iN > dpsNorth.seconds)
        {
            dpsNorth = Geo.shift(dpsNorth, 10);
        }
        else
        {
            dpssouth = Geo.shift(dpssouth, -10);
        }

        if (iW < dpswest.seconds)
        {
            dpswest = Geo.shift(dpswest, -10);
        }
        else
        {
            dpseast = Geo.shift(dpseast, 10);
        }

        box.north = Math.Round(Geo.dpsToDeg(dpsNorth), 5);
        box.west = Math.Round(Geo.dpsToDeg(dpswest), 5);
        box.south = Math.Round(Geo.dpsToDeg(dpssouth), 5);
        box.east = Math.Round(Geo.dpsToDeg(dpseast), 5);

        return box;
    }

    public static Geo.Dps shift(Geo.Dps dps, int seconds)
    {

        if (seconds < 0 && dps.seconds <= Math.Abs(seconds) - 1)
        {
            //if going down, and ther are not enough seconds, take it out of primes
            dps.seconds = (60 - Math.Abs(seconds)) + (dps.seconds % seconds);
            dps.primes -= 1;
        }
        else if (seconds > 0 && dps.seconds >= (59 - Math.Abs(seconds)))
        {
            dps.seconds = dps.seconds % (60 - seconds);
            dps.primes += 1;
        }
        else
        {
            dps.seconds += seconds;
        }

        return dps;
    }
    public struct Dps
    {
        public int degrees;
        public int primes;
        public int seconds;
    }

    public struct Box
    {
        public double north;
        public double south;
        public double east;
        public double west;
    }

    public struct LatLng
    {
        public double lat;
        public double lng;
    }
}