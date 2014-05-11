namespace OpenTrackLogger.Services
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Xml;

    using Microsoft.Phone.Reactive;

    using OpenTrackLogger.Models;

    public class TrackExportProgress
    {
        public int WaypointsWritten { get; private set; }
        public int TotalWaypoints { get; private set; }

        public int TrackpointsWritten { get; private set; }
        public int TotalTrackpoints { get; private set; }

        public int NodesWritten { get; private set; }
        public int TotalNodes { get; private set; }
        public double PercentageComplete { get; private set; }

        internal TrackExportProgress(int waypointsWritten, int totalWaypoints, int trackpointsWritten, int totalTrackpoints)
        {
            WaypointsWritten = waypointsWritten;
            TotalWaypoints = totalWaypoints;
            TrackpointsWritten = trackpointsWritten;
            TotalTrackpoints = totalTrackpoints;
            NodesWritten = waypointsWritten + trackpointsWritten;
            TotalNodes = totalWaypoints + TotalTrackpoints;
            PercentageComplete = NodesWritten / (double)TotalNodes * 100;
        }
    }

    public interface ITrackExportService
    {
        Task WriteTrackToStream(TrackSummary trackSummary, Stream writeStream);
        IObservable<TrackExportProgress> TrackExportProgress { get; }
    }

    public class TrackExportService : ITrackExportService
    {
        private const string _ns = "http://www.topografix.com/GPX/1/1";
        private const string _nsXsi = "http://www.w3.org/2001/XMLSchema-instance";

        public TrackExportService()
        {
            _trackExportProgress = new Subject<TrackExportProgress>();    
        }

        public async Task WriteTrackToStream(TrackSummary trackSummary, Stream writeStream)
        {
            using (var xmlWriter = XmlWriter.Create(writeStream, new XmlWriterSettings { Async = true })) {
                await xmlWriter.WriteStartDocumentAsync(false);

                await xmlWriter.WriteStartElementAsync(null, "gpx", _ns);
                await xmlWriter.WriteAttributeStringAsync(null, "version", null, "1.1");
                await xmlWriter.WriteAttributeStringAsync(null, "creator", null, "OpenTrackLogger for Windows Phone - TODO: ADD URL");
                await xmlWriter.WriteAttributeStringAsync("xmlns", "xsi", null, _nsXsi);
                await xmlWriter.WriteAttributeStringAsync("xsi", "schemaLocation", null, "http://www.topografix.com/GPX/1/1 http://www.topografix.com/GPX/1/1/gpx.xsd");

                using (var db = new OpenTrackLoggerDataContext(OpenTrackLoggerDataContext.ConnectionString)) {
                    var waypoints = db.Waypoints
                        .Where(x => x.TrackId == trackSummary.Track.Id)
                        .Join(db.Trackpoints, p => p.TrackpointId, tp => tp.Id, (p, tp) => p)
                        .OrderBy(p => p.CreatedAt);

                    var waypointIndex = 0;

                    foreach (var waypoint in waypoints) {
                        await xmlWriter.WriteStartElementAsync(null, "wpt", _ns);

                        await xmlWriter.WriteAttributeStringAsync(null, "lat", null, waypoint.Trackpoint.Latitude.ToString());
                        await xmlWriter.WriteAttributeStringAsync(null, "lon", null, waypoint.Trackpoint.Longitude.ToString());

                        if (waypoint.Trackpoint.Elevation.HasValue && !double.IsNaN(waypoint.Trackpoint.Elevation.Value)) {
                            await xmlWriter.WriteElementStringAsync(null, "ele", _ns, waypoint.Trackpoint.Elevation.Value.ToString());
                        }

                        await xmlWriter.WriteElementStringAsync(null, "time", _ns, waypoint.CreatedAt.ToString("s") + "Z");

                        await xmlWriter.WriteStartElementAsync(null, "name", _ns);
                        await xmlWriter.WriteCDataAsync("Picture");
                        await xmlWriter.WriteEndElementAsync(); // name

                        await xmlWriter.WriteStartElementAsync(null, "link", _ns);
                        await xmlWriter.WriteAttributeStringAsync(null, "href", null, waypoint.Link);
                        await xmlWriter.WriteElementStringAsync(null, "name", _ns, waypoint.Link);
                        await xmlWriter.WriteEndElementAsync(); // link

                        await xmlWriter.WriteEndElementAsync(); // wpt

                        _trackExportProgress.OnNext(new TrackExportProgress(++waypointIndex, trackSummary.NumberOfWaypoints, 0, trackSummary.NumberOfTrackpoints));
                    }
                }

                using (var db = new OpenTrackLoggerDataContext(OpenTrackLoggerDataContext.ConnectionString)) {
                    var trackpoints = db.Trackpoints
                        .Where(x => x.TrackId == trackSummary.Track.Id)
                        .OrderBy(x => x.Timestamp);

                    var trackpointIndex = 0;

                    await xmlWriter.WriteStartElementAsync(null, "trk", _ns);

                    await xmlWriter.WriteStartElementAsync(null, "name", _ns);
                    await xmlWriter.WriteCDataAsync("Tracked with OpenTrackLogger for Windows Phone");
                    await xmlWriter.WriteEndElementAsync(); // name

                    await xmlWriter.WriteStartElementAsync(null, "trkseg", _ns);

                    foreach (var trackpoint in trackpoints) {
                        await xmlWriter.WriteStartElementAsync(null, "trkpt", _ns);
                        await xmlWriter.WriteAttributeStringAsync(null, "lat", null, trackpoint.Latitude.ToString());
                        await xmlWriter.WriteAttributeStringAsync(null, "lon", null, trackpoint.Longitude.ToString());

                        if (trackpoint.Elevation.HasValue && !double.IsNaN(trackpoint.Elevation.Value)) {
                            await xmlWriter.WriteElementStringAsync(null, "ele", _ns, trackpoint.Elevation.Value.ToString());
                        }

                        await xmlWriter.WriteElementStringAsync(null, "time", _ns, trackpoint.Timestamp.ToString("s") + "Z");

                        if (trackpoint.Speed.HasValue && !double.IsNaN(trackpoint.Speed.Value)) {
                            await xmlWriter.WriteStartElementAsync(null, "extensions", _ns);
                            await xmlWriter.WriteElementStringAsync(null, "speed", _ns, trackpoint.Speed.Value.ToString());
                            await xmlWriter.WriteEndElementAsync(); // extensions
                        }

                        await xmlWriter.WriteEndElementAsync(); // trkpt

                        _trackExportProgress.OnNext(new TrackExportProgress(trackSummary.NumberOfWaypoints, trackSummary.NumberOfWaypoints, ++trackpointIndex, trackSummary.NumberOfTrackpoints));
                    }

                    await xmlWriter.WriteEndElementAsync(); // trkseg
                    await xmlWriter.WriteEndElementAsync(); // trk
                }


                await xmlWriter.WriteEndElementAsync(); // gpx

                await xmlWriter.WriteEndDocumentAsync();
            }
        }

        private readonly Subject<TrackExportProgress> _trackExportProgress;
        public IObservable<TrackExportProgress> TrackExportProgress { get { return _trackExportProgress; } }
    }
}
