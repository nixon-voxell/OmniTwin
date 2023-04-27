using System.Xml;
using Unity.Mathematics;

namespace OpenStreetMap
{
    public struct OSMNode
    {
        public ulong Id;

        /// <summary>Longitude and latitude.</summary>
        public float2 GeoCoordinate;
        /// <summary>XY mercator projection.</summary>
        public float2 Coordinate;

        public OSMNode(XmlNode node)
        {
            this.Id = OSMUtil.GetAttribute<ulong>("id", node.Attributes);

            float longitude = OSMUtil.GetAttribute<float>("lon", node.Attributes);
            float latitude = OSMUtil.GetAttribute<float>("lat", node.Attributes);
            this.GeoCoordinate = new float2(longitude, latitude);

            this.Coordinate = OSMUtil.MercatorProjection.ToCoordinate(this.GeoCoordinate);
        }
    }
}
