namespace OmniTwin
{
    public struct DisasterData
    {
        public double Latitude;
        public double Longitude;
        public string DetectionImageURL;
        public string Category;

        public DisasterData(double lat, double lon, string imageURL, string category)
        {
            this.Latitude = lat;
            this.Longitude = lon;
            this.DetectionImageURL = imageURL;
            this.Category = category;
        }
    }
}
