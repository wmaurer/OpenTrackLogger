namespace TestClient
{
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data.Entity;
    using System.Data.SqlServerCe;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;

    class Program
    {
        static void Main()
        {
            //var engine = new SqlCeEngine(ConfigurationManager.ConnectionStrings["OpenTrackLogger"].ConnectionString);
            //engine.Upgrade(ConfigurationManager.ConnectionStrings["OpenTrackLogger"].ConnectionString);
            using (var dbContext = new OpenTrackLoggerDbContext(ConfigurationManager.ConnectionStrings["OpenTrackLogger"].ConnectionString)) {
                var trackpoints = dbContext.Database.SqlQuery<Trackpoint>("select * from Trackpoint  where TrackId = 8 or TrackId = 9 or TrackId = 10 order by TrackPointId").ToList();
                var waypoints = dbContext.Database.SqlQuery<Waypoint>("select * from Waypoint where TrackId = 8 or TrackId = 9 or TrackId = 10 order by WaypointId").ToList();
                WriteTrackToStream(waypoints, trackpoints);
            }
        }

        private const string _ns = "http://www.topografix.com/GPX/1/1";
        private const string _nsXsi = "http://www.w3.org/2001/XMLSchema-instance";

        public static void WriteTrackToStream(List<Waypoint> waypoints, List<Trackpoint> trackpoints)
        {
            Stream writeStream = new FileStream(@"D:\other\osmtracker\20140609\20140609.gpx", FileMode.Create);
            using (var xmlWriter = XmlWriter.Create(writeStream, new XmlWriterSettings { Async = false })) {
                xmlWriter.WriteStartDocument(false);

                xmlWriter.WriteStartElement("gpx", _ns);
                xmlWriter.WriteAttributeString("version", "1.1");
                xmlWriter.WriteAttributeString("creator", "OpenTrackLogger for Windows Phone - TODO: ADD URL");
                //xmlWriter.WriteAttributeString("xmlns", "xsi", _nsXsi);
                xmlWriter.WriteAttributeString("xsi", "schemaLocation", "http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd");

                foreach (var waypoint in waypoints) {
                    xmlWriter.WriteStartElement("wpt", _ns);

                    var trackpoint = trackpoints.First(x => x.TrackpointId == waypoint.TrackpointId);
                    xmlWriter.WriteAttributeString("lat", trackpoint.Latitude.ToString());
                    xmlWriter.WriteAttributeString("lon", trackpoint.Longitude.ToString());

                    xmlWriter.WriteElementString("time", _ns, waypoint.CreatedAt.ToString("s") + "Z");

                    xmlWriter.WriteStartElement("name", _ns);
                    xmlWriter.WriteCData("Picture");
                    xmlWriter.WriteEndElement(); // name

                    xmlWriter.WriteStartElement("link", _ns);
                    xmlWriter.WriteAttributeString("href", waypoint.Link);
                    xmlWriter.WriteElementString("name", _ns, waypoint.Link);
                    xmlWriter.WriteEndElement(); // link

                    xmlWriter.WriteEndElement(); // wpt
                }

                xmlWriter.WriteStartElement("trk", _ns);

                xmlWriter.WriteStartElement("name", _ns);
                xmlWriter.WriteCData("Tracked with OpenTrackLogger for Windows Phone");
                xmlWriter.WriteEndElement(); // name

                xmlWriter.WriteStartElement("trkseg", _ns);

                foreach (var trackpoint in trackpoints) {
                    xmlWriter.WriteStartElement("trkpt", _ns);
                    xmlWriter.WriteAttributeString("lat", trackpoint.Latitude.ToString());
                    xmlWriter.WriteAttributeString("lon", trackpoint.Longitude.ToString());

                    xmlWriter.WriteElementString("time", _ns, trackpoint.Timestamp.ToString("s") + "Z");

                    xmlWriter.WriteEndElement(); // trkpt

                }

                xmlWriter.WriteEndElement(); // trkseg
                xmlWriter.WriteEndElement(); // trk


                xmlWriter.WriteEndElement(); // gpx

                xmlWriter.WriteEndDocument();
            }
        }
    }
}
