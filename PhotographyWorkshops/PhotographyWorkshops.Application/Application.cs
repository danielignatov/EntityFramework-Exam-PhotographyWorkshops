namespace PhotographyWorkshops.Application
{
    using Data;
    using Models;
    using Models.DTO;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Xml.Linq;
    using System.Xml.XPath;

    class Application
    {
        private const string ImportCamerasPath = @"../../../Resources/cameras.json";
        private const string ImportLensesPath = @"../../../Resources/lenses.json";
        private const string ImportPhotographersPath = @"../../../Resources/photographers.json";
        private const string ImportAccessoriesPath = @"../../../Resources/accessories.xml";
        private const string ImportWorkshopsPath = @"../../../Resources/workshops.xml";

        static void Main(string[] args)
        {
            var context = new PhotographyWorkshopsContext();

            // JSON Imports
            ImportCamerasFromJSON(context);
            ImportLensesFromJSON(context);
            ImportPhotographersFromJSON(context);
            
            // XML Imports
            ImportAccessoriesFromXML(context);
            //ImportWorkshopsFromXML(context); // Throw Exception
            
            // JSON Exports
            ExportOrderedPhotographersToJSON(context);
            ExportLandscapePhotographersToJSON(context);

            // XML Exports
            ExportPhotographersWithSameCameraMakeToXML(context);
            //ExportWorkshopsByLocationToXML(context); // Unfinished
        }

        private static void ExportWorkshopsByLocationToXML(PhotographyWorkshopsContext context)
        {
            var workshopsByLocation = context.Workshops
                .Where(p => p.Participants.Count >= 5)
                .GroupBy(l => l.Location);
                

            var xmlDocument = new XDocument();
            var xmlLocations = new XElement("locations");

            foreach (var location in workshopsByLocation)
            {
                var locationNode = new XElement("location");
                locationNode.Add(new XAttribute("name", location));
                
                // Unfinished
            }

            xmlDocument.Add(xmlLocations);
            xmlDocument.Save($"../../ workshops-by-location.xml");

            Console.WriteLine(xmlDocument);
        }

        private static void ExportPhotographersWithSameCameraMakeToXML(PhotographyWorkshopsContext context)
        {
            var photographersWithSameCameraMake = context.Photographers
                .Where(c => c.PrimaryCamera.Make == c.SecondaryCamera.Make);
        
            var xmlDocument = new XDocument();
            var xmlPhotographers = new XElement("photographers");
        
            foreach (var photographer in photographersWithSameCameraMake)
            {
                var photographerNode = new XElement("photographer");
                photographerNode.Add(new XAttribute("name", photographer.FirstName + " " + photographer.LastName));
                photographerNode.Add(new XAttribute("primary-camera", photographer.PrimaryCamera.Make + " " + photographer.PrimaryCamera.Model));

                var xmlLenses = new XElement("lenses");

                foreach (var lens in photographer.Lenses)
                {
                    var lense = new XElement("lens", $"{lens.Make} {lens.FocalLength}mm f{lens.MaxAperture}");
                    xmlLenses.Add(lense);
                }

                photographerNode.Add(xmlLenses);
                xmlPhotographers.Add(photographerNode);
            }
        
            xmlDocument.Add(xmlPhotographers);
            xmlDocument.Save($"../../same-cameras-photographers.xml");
        
            Console.WriteLine(xmlDocument);
        }

        private static void ExportLandscapePhotographersToJSON(PhotographyWorkshopsContext context)
        {
            var landscapePhotographers = context.Photographers
                .OrderBy(fn => fn.FirstName)
                .Where(pc => pc.PrimaryCamera.Type == "DSLR")
                .Where(l => l.Lenses.All(fl => fl.FocalLength <= 30f))
                .Where(lc => lc.Lenses.Count() > 0)
                .Select(p => new
                {
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    CameraMake = p.PrimaryCamera.Make,
                    LensesCount = p.Lenses.Count
                });

            var json = JsonConvert.SerializeObject(landscapePhotographers, Formatting.Indented);
            File.WriteAllText($"../../landscape-photogaphers.json", json);
            Console.WriteLine(json);
        }

        private static void ExportOrderedPhotographersToJSON(PhotographyWorkshopsContext context)
        {
            var photographersOrdered = context.Photographers
                .OrderBy(fn => fn.FirstName)
                .ThenByDescending(ln => ln.LastName)
                .Select(p => new
                {
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    Phone = p.Phone
                });

            var json = JsonConvert.SerializeObject(photographersOrdered, Formatting.Indented);
            File.WriteAllText($"../../photographers-ordered.json", json);
            Console.WriteLine(json);
        }

        private static void ImportWorkshopsFromXML(PhotographyWorkshopsContext context)
        {
            var xml = XDocument.Load(ImportWorkshopsPath);
            var workshops = xml.XPathSelectElements("workshops/workshop");

            foreach (var workshop in workshops)
            {
                var workshopName = workshop.Attribute("name");
                var workshopStartDate = workshop.Attribute("start-date");
                var workshopEndDate = workshop.Attribute("end-date");
                var workshopLocation = workshop.Attribute("location");
                var workshopPrice = workshop.Attribute("price");
                var workshopTrainer = workshop.Element("trainer");

                if ((workshopName == null) ||
                    (workshopLocation == null) ||
                    (workshopPrice == null) ||
                    (workshopTrainer == null))
                {
                    Console.WriteLine("Error. Invalid data provided");
                    continue;
                }

                string[] workshopTrainerNames = workshopTrainer.Value.Trim().Split(' ').ToArray();
                string workshopTrainerFirstName = workshopTrainerNames[0].ToString();
                string workshopTrainerLastName = workshopTrainerNames[1].ToString();

                var workshopParticipants = workshop.XPathSelectElements("participants/participant");
                HashSet<Photographer> participants = new HashSet<Photographer>();

                foreach (var workshopParticipant in workshopParticipants)
                {
                    string firstName = workshopParticipant.Attribute("first-name").Value;
                    string lastName = workshopParticipant.Attribute("last-name").Value;

                    var participant = context.Photographers
                        .Where(fn => fn.FirstName == firstName)
                        .Where(ln => ln.LastName == lastName)
                        .FirstOrDefault();

                    if (participant == null)
                    {
                        continue;
                    }

                    participants.Add(participant);
                }

                var workshopEntity = new Workshop()
                {
                    Name = workshopName.Value,
                    StartDate = IfNoDateTimeValueReturnNull(workshopStartDate.Value),
                    EndDate = IfNoDateTimeValueReturnNull(workshopEndDate.Value),
                    Location = workshopLocation.Value,
                    PricePerParticipant = decimal.Parse(workshopPrice.Value),
                    Trainer = context.Photographers.Where(n => n.FirstName == workshopTrainerFirstName)
                    .Where(ln => ln.LastName == workshopTrainerLastName).FirstOrDefault(),
                    Participants = participants
                };

                context.Workshops.Add(workshopEntity);
                Console.WriteLine($"Successfully imported {workshopName.Value}");
            }

            context.SaveChanges();
        }

        private static void ImportAccessoriesFromXML(PhotographyWorkshopsContext context)
        {
            var xml = XDocument.Load(ImportAccessoriesPath);
            var accessories = xml.XPathSelectElements("accessories/accessory");
            Random rng = new Random();

            foreach (var accessory in accessories)
            {
                var accessoryName = accessory.Attribute("name");

                if ((String.IsNullOrWhiteSpace(accessoryName.Value)) ||
                        (accessoryName == null))
                {
                    Console.WriteLine("Error. Invalid data provided");
                    continue;
                }

                int randomPhotographerId = rng.Next(1, context.Photographers.Count());

                var accessoryEntity = new Accessory()
                {
                    Name = accessoryName.Value,
                    Owner = context.Photographers.Where(i => i.Id == randomPhotographerId).SingleOrDefault()
                };

                context.Accessories.Add(accessoryEntity);
                Console.WriteLine($"Successfully imported {accessoryName.Value}");
            }

            context.SaveChanges();
        }

        private static void ImportPhotographersFromJSON(PhotographyWorkshopsContext context)
        {
            var json = File.ReadAllText(ImportPhotographersPath);
            var photographers = JsonConvert.DeserializeObject<IEnumerable<PhotographerDTO>>(json);
            Random rng = new Random();

            foreach (var photographer in photographers)
            {
                if ((photographer.FirstName == null) ||
                    (photographer.LastName == null) ||
                    (photographer.Phone == null) ||
                    (photographer.Lenses == null))
                {
                    Console.WriteLine("Error. Invalid data provided");
                    continue;
                }

                HashSet<Lens> lenses = new HashSet<Lens>();

                foreach (var lensId in photographer.Lenses)
                {
                    var lens = context.Lenses.Where(i => i.Id == lensId).SingleOrDefault();

                    if (lens == null)
                    {
                        continue;
                    }

                    lenses.Add(lens);
                }

                var photographerEntity = new Photographer()
                {
                    FirstName = photographer.FirstName,
                    LastName = photographer.LastName,
                    Phone = photographer.Phone,
                    Lenses = lenses,
                    PrimaryCamera = context.Cameras.Find(rng.Next(1, context.Cameras.Count())),
                    SecondaryCamera = context.Cameras.Find(rng.Next(1, context.Cameras.Count()))
                };

                context.Photographers.Add(photographerEntity);
                Console.WriteLine($"Successfully imported {photographer.FirstName} {photographer.LastName} | {lenses.Count()}");
            }

            context.SaveChanges();
        }

        private static void ImportLensesFromJSON(PhotographyWorkshopsContext context)
        {
            var json = File.ReadAllText(ImportLensesPath);
            var lenses = JsonConvert.DeserializeObject<IEnumerable<LensDTO>>(json);

            foreach (var lens in lenses)
            {
                if ((lens.Make == null) ||
                    (lens.FocalLength == null) ||
                    (lens.MaxAperture == null) ||
                    (lens.CompatibleWith == null))
                {
                    Console.WriteLine("Error. Invalid data provided");
                    continue;
                }

                var lensEntity = new Lens()
                {
                    Make = lens.Make,
                    FocalLength = IfNoIntValueReturnNull(lens.FocalLength),
                    MaxAperture = IfNoFloatValueReturnNull(lens.MaxAperture),
                    CompatibleWith = lens.CompatibleWith
                };

                context.Lenses.Add(lensEntity);
                Console.WriteLine($"Successfully imported {lens.Make} {lens.FocalLength}mm f{lens.MaxAperture}");
            }

            context.SaveChanges();
        }

        private static DateTime? IfNoDateTimeValueReturnNull(string value)
        {
            try
            {
                return DateTime.Parse(value);
            }
            catch
            {
                return null;
            }
        }

        private static float? IfNoFloatValueReturnNull(string value)
        {
            try
            {
                return float.Parse(value);
            }
            catch
            {
                return null;
            }
        }

        private static void ImportCamerasFromJSON(PhotographyWorkshopsContext context)
        {
            var json = File.ReadAllText(ImportCamerasPath);
            var cameras = JsonConvert.DeserializeObject<IEnumerable<CameraDTO>>(json);

            foreach (var camera in cameras)
            {
                if ((camera.Type == null) ||
                    (camera.Make == null) ||
                    (camera.Model == null) ||
                    (camera.MinISO == null))
                {
                    Console.WriteLine("Error. Invalid data provided");
                    continue;
                }

                if (int.Parse(camera.MinISO) < 100)
                {
                    camera.MinISO = null;
                }

                if (camera.Type == "DSLR")
                {
                    var cameraEntity = new Camera()
                    {
                        Type = camera.Type,
                        Make = camera.Make,
                        Model = camera.Model,
                        MinISO = IfNoIntValueReturnNull(camera.MinISO),
                        MaxISO = IfNoIntValueReturnNull(camera.MaxISO),
                        MaxShutterSpeed = IfNoIntValueReturnNull(camera.MaxShutterSpeed)
                    };

                    context.Cameras.Add(cameraEntity);
                    Console.WriteLine($"Successfully imported {camera.Type} {camera.Make} {camera.Model}");
                }
                else if (camera.Type == "Mirrorless")
                {
                    var cameraEntity = new Camera()
                    {
                        Type = camera.Type,
                        Make = camera.Make,
                        Model = camera.Model,
                        IsFullFrame = IfNoBoolValueReturnNull(camera.IsFullFrame),
                        MinISO = IfNoIntValueReturnNull(camera.MinISO),
                        MaxISO = IfNoIntValueReturnNull(camera.MaxISO),
                        MaxVideoResolution = camera.MaxVideoResolution,
                        MaxFrameRate = IfNoIntValueReturnNull(camera.MaxFrameRate)
                    };

                    context.Cameras.Add(cameraEntity);
                    Console.WriteLine($"Successfully imported {camera.Type} {camera.Make} {camera.Model}");
                }
            }

            context.SaveChanges();
        }



        private static bool? IfNoBoolValueReturnNull(string value)
        {
            try
            {
                return bool.Parse(value);
            }
            catch
            {
                return null;
            }
        }

        private static int? IfNoIntValueReturnNull(string value)
        {
            try
            {
                return int.Parse(value);
            }
            catch
            {
                return null;
            }
        }
    }
}