using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;
using GeoJSON.Net;
using GeoJSON.Net.Feature;
using GeoJSON.Net.Geometry;

namespace AirportExplorer.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IHostingEnvironment hostingEnvironment;

        public string MapboxAccessToken { get; }

        public IndexModel(IConfiguration configuration, IHostingEnvironment _hostingEnvironment)
        {
            MapboxAccessToken = configuration["MapBox:AccessToken"];
            this.hostingEnvironment = _hostingEnvironment;
        }

        

        public void OnGet()
        {

        }

        public IActionResult OnGetAirports()
        {
            var configuration = new Configuration
            {
                BadDataFound = context =>
                {

                }
            };

            using (var sr = new StreamReader(Path.Combine(hostingEnvironment.WebRootPath, "airports.dat"))) //gets the physical location of wwwroot folder
            using (var reader = new CsvReader(sr,configuration)) //skips the double quote escape characters in the city
            {
                FeatureCollection featureCollection = new FeatureCollection();

                while (reader.Read())
                {
                    string name = reader.GetField<string>(1);
                    string iataCode = reader.GetField<string>(4);
                    double latitude = reader.GetField<double>(6);
                    double longitude = reader.GetField<double>(7);

                    featureCollection.Features.Add(new Feature(
                        new Point(new Position(latitude, longitude)),
                        new Dictionary<string, object>
                        {
                            {"name",name },
                            {"iatacode",iataCode}
                        }
                        ));

                }

                return new JsonResult(featureCollection);
            }
        }
    }
}
