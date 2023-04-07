using System.Xml;
using Unity.Mathematics;

namespace OpenStreetMap
{
    public static class OSMUtil
    {
        public static T GetAttribute<T>(string attrName, XmlAttributeCollection attributes)
        {
            // TODO: we are going to assume that `attrName` exists in the collection
            string strValue = attributes[attrName].Value;
            return (T)System.Convert.ChangeType(strValue, typeof(T));
        }

        public static class MercatorProjection
        {
            private const float R_MAJOR = 6378137.0f;
            private const float R_MINOR = 6356752.3142f;
            private static readonly float RATIO = R_MINOR / R_MAJOR;
            private static readonly float ECCENT = math.sqrt(1.0f - (RATIO * RATIO));
            private static readonly float COM = 0.5f * ECCENT;

            private static readonly float PI_2 = math.PI / 2.0f;

            public static float2 ToCoordinate(float2 geoCoord)
            {
                return new float2(LonToX(geoCoord.x), LatToY(geoCoord.y));
            }

            public static float2 ToGeoCoord(float2 coord)
            {
                return new float2(XToLon(coord.x), YToLat(coord.y));
            }

            public static float LonToX(float lon)
            {
                return R_MAJOR * math.radians(lon);
            }

            public static float LatToY(float lat)
            {
                lat = math.min(89.5f, math.max(lat, -89.5f));
                float phi = math.radians(lat);
                float sinphi = math.sin(phi);
                float con = ECCENT * sinphi;
                con = math.pow(((1.0f - con) / (1.0f + con)), COM);
                float ts = math.tan(0.5f * ((math.PI * 0.5f) - phi)) / con;
                return 0 - R_MAJOR * math.log(ts);
            }

            public static float XToLon(float x)
            {
                return math.degrees(x) / R_MAJOR;
            }

            public static float YToLat(float y)
            {
                float ts = math.exp(-y / R_MAJOR);
                float phi = PI_2 - 2 * math.atan(ts);
                float dphi = 1.0f;
                int i = 0;

                while ((math.abs(dphi) > math.EPSILON) && (i < 15))
                {
                    float con = ECCENT * math.sin(phi);
                    dphi = PI_2 - 2 * math.atan(ts * math.pow((1.0f - con) / (1.0f + con), COM)) - phi;
                    phi += dphi;
                    i++;
                }

                return math.degrees(phi);
            }
        }
    }
}
