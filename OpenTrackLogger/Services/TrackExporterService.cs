namespace OpenTrackLogger.Services
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml;
    using System.Xml.Linq;

    using OpenTrackLogger.Models;

    public static class TrackExporterService
    {
        private const string _ns = "http://www.topografix.com/GPX/1/1";
        private const string _nsXsi = "http://www.w3.org/2001/XMLSchema-instance";

        public static void WriteToStream(this Track track, Stream writeStream)
        {
            using (var xmlWriter = XmlWriter.Create(writeStream, new XmlWriterSettings { Async = true })) {
                xmlWriter.WriteStartDocumentAsync(false);

                xmlWriter.WriteStartElementAsync(null, "gpx", _ns);
                xmlWriter.WriteAttributeStringAsync(null, "version", null, "1.1");
                xmlWriter.WriteAttributeStringAsync(null, "creator", null, "OpenTrackLogger for Windows Phone - TODO: ADD URL");
                xmlWriter.WriteAttributeStringAsync("xmlns", "xsi", null, _nsXsi);
                xmlWriter.WriteAttributeStringAsync("xsi", "schemaLocation", null, "http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd");


                using (var db = new OpenTrackLoggerDataContext(OpenTrackLoggerDataContext.ConnectionString)) {
                    var photos = db.Photos
                        .Where(x => x.TrackId == track.Id)
                        .Join(db.Trackpoints, p => p.TrackpointId, tp => tp.Id, (p, tp) => p)
                        .OrderBy(p => p.CreatedAt);

                    foreach (var photo in photos) {
                        xmlWriter.WriteStartElementAsync(null, "wpt", _ns);

                        xmlWriter.WriteAttributeStringAsync(null, "lat", null, photo.Trackpoint.Latitude.ToString());
                        xmlWriter.WriteAttributeStringAsync(null, "lon", null, photo.Trackpoint.Longitude.ToString());

                        if (photo.Trackpoint.Elevation.HasValue) {
                            xmlWriter.WriteElementStringAsync(null, "ele", _ns, photo.Trackpoint.Elevation.ToString());
                        }

                        xmlWriter.WriteElementStringAsync(null, "time", _ns, photo.CreatedAt.ToString("s") + "Z");

                        xmlWriter.WriteStartElementAsync(null, "name", _ns);
                        xmlWriter.WriteCDataAsync("Picture");
                        xmlWriter.WriteEndElementAsync(); // name

                        xmlWriter.WriteStartElementAsync(null, "link", _ns);
                        xmlWriter.WriteAttributeStringAsync(null, "href", null, photo.Filename);
                        xmlWriter.WriteElementStringAsync(null, "name", _ns, photo.Filename);
                        xmlWriter.WriteEndElementAsync(); // link

                        xmlWriter.WriteEndElementAsync(); // wpt
                    }

                    var trackpoints = db.Trackpoints
                        .Where(x => x.TrackId == track.Id)
                        .OrderBy(x => x.Timestamp);

                    xmlWriter.WriteStartElementAsync(null, "trk", _ns);

                    xmlWriter.WriteStartElementAsync(null, "name", _ns);
                    xmlWriter.WriteCDataAsync("Tracked with OpenTrackLogger for Windows Phone");
                    xmlWriter.WriteEndElementAsync(); // name

                    xmlWriter.WriteStartElementAsync(null, "trkseg", _ns);


                    foreach (var trackpoint in trackpoints) {
                        xmlWriter.WriteStartElementAsync(null, "trkpt", _ns);
                        xmlWriter.WriteAttributeStringAsync(null, "lat", null, trackpoint.Latitude.ToString());
                        xmlWriter.WriteAttributeStringAsync(null, "lon", null, trackpoint.Longitude.ToString());

                        if (trackpoint.Elevation.HasValue) {
                            xmlWriter.WriteElementStringAsync(null, "ele", _ns, trackpoint.Elevation.ToString());
                        }

                        xmlWriter.WriteElementStringAsync(null, "time", _ns, trackpoint.Timestamp.ToString("s") + "Z");

                        if (trackpoint.Speed.HasValue) {
                            xmlWriter.WriteStartElementAsync(null, "extensions", _ns);
                            xmlWriter.WriteElementStringAsync(null, "speed", _ns, trackpoint.Speed.ToString());
                            xmlWriter.WriteEndElementAsync(); // extensions
                        }

                        xmlWriter.WriteEndElementAsync(); // trkpt
                    }

                    xmlWriter.WriteEndElementAsync(); // trkseg
                    xmlWriter.WriteEndElementAsync(); // trk
                }


                xmlWriter.WriteEndElementAsync(); // gpx

                xmlWriter.WriteEndDocumentAsync();
            }
        }
    }
}
