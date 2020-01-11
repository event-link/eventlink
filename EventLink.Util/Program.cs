using EventLink.API.Services;
using EventLink.DataAccess.Models;
using EventLink.Util.Util;
using System;
using System.Collections.Generic;
using System.Linq;

namespace EventLink.Util
{
    class Program
    {
        private static readonly IUserService UserService = API.Services.UserService.Instance;
        private static readonly IEventService EventService = API.Services.EventService.Instance;

        static void Main(string[] args)
        {
            CreateAdminUsers();
        }

        private static void CreateAdminUsers()
        {
            Console.WriteLine("Creating users...");

            var daniel = new User(AccountType.Admin, LoginMethod.Eventlink, "picUrl", "Daniel", "", "Larsen", "Daniel Larsen", "iyyelsec@gmail.com", "address", DateTime.Now, "password", "12345678", "Denmark",
                new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), DateTime.Now, true);
            var thomas = new User(AccountType.Admin, LoginMethod.Eventlink, "picUrl", "Thomas", "", "Mascagni", "Thomas Mascagni", "t.mascagni@gmail.com", "address", DateTime.Now, "death", "87654321", "Denmark",
                new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), DateTime.Now, true);
            var emma = new User(AccountType.Regular, LoginMethod.Eventlink, "picUrl", "Emma", "Moorman", "Franklin", "Emma Moorman Franklin", "emma@eventlink.ml", "address", DateTime.Now, "emma", "00000000", "United States",
                new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), DateTime.Now, true);
            var rose = new User(AccountType.Regular, LoginMethod.Eventlink, "picUrl", "Rose", "Ramirez", "Lemmons", "Rose Ramirez Lemmons", "rose@eventlink.ml", "address", DateTime.Now, "rose", "00000000", "Mexico",
                new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), DateTime.Now, true);
            var clayton = new User(AccountType.Regular, LoginMethod.Eventlink, "picUrl", "Clayton", "Hicks", "Martin", "Clayton Hicks Martin", "clayton@eventlink.ml", "address", DateTime.Now, "clayton", "00000000", "England",
                new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), DateTime.Now, true);
            var meow = new User(AccountType.Regular, LoginMethod.Eventlink, "picUrl", "Mr. Meow", "", "", "Mr. Meow", "meow@eventlink.ml", "address", DateTime.Now, "meow", "00000000", "Meowton",
                new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), DateTime.Now, true);
            var stephanov = new User(AccountType.Regular, LoginMethod.Eventlink, "picUrl", "Stephanov", "", "Shaposhnikov", "Stephanov Shaposhnikov", "stephanov@eventlink.ml", "address", DateTime.Now, "stephanov", "00000000", "Russia",
                new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), DateTime.Now, true);
            var harold = new User(AccountType.Regular, LoginMethod.Eventlink, "picUrl", "Harold", "Annie", "Weatherly", "Harold Annie Weatherly", "harold@eventlink.ml", "address", DateTime.Now, "harold", "00000000", "United States",
                new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), DateTime.Now, true);
            var alicia = new User(AccountType.Regular, LoginMethod.Eventlink, "picUrl", "Alicia", "Boyle", "Pfeil", "Alicia Boyle Pfeil", "alicia@eventlink.ml", "address", DateTime.Now, "alicia", "00000000", "Peru",
                new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), DateTime.Now, true);
            var howard = new User(AccountType.Regular, LoginMethod.Eventlink, "picUrl", "Howard", "Mane", "Sutton", "Howard Mane Sutton", "howard@eventlink.ml", "address", DateTime.Now, "howard", "00000000", "Spain",
               new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), DateTime.Now, true);
            var jeffrey = new User(AccountType.Regular, LoginMethod.Eventlink, "picUrl", "Jeffrey", "Brown", "Trinidad", "Jeffrey Brown Trinidad", "jeffrey@eventlink.ml", "address", DateTime.Now, "jeffrey", "00000000", "United States",
                new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), DateTime.Now, true);
            var john = new User(AccountType.Regular, LoginMethod.Eventlink, "picUrl", "John", "", "McNeilly", "John McNeilly", "john@eventlink.ml", "address", DateTime.Now, "john", "00000000", "Ireland",
                new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), DateTime.Now, true);
            var kenneth = new User(AccountType.Regular, LoginMethod.Eventlink, "picUrl", "Kenneth", "", "Montgomery", "Kenneth Montgomery", "kenneth@eventlink.ml", "address", DateTime.Now, "kenneth", "00000000", "Germany",
                new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), DateTime.Now, true);
            var miriam = new User(AccountType.Regular, LoginMethod.Eventlink, "picUrl", "Miriam", "", "Schneider", "Miriam Schneider", "miriam@eventlink.ml", "address", DateTime.Now, "miriam", "00000000", "Sweden",
                new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), DateTime.Now, true);
            var dennis = new User(AccountType.Regular, LoginMethod.Eventlink, "picUrl", "Dennis", "T", "Engels", "Dennis T Engels", "dennis@eventlink.ml", "address", DateTime.Now, "dennis", "00000000", "Denmark",
                new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), DateTime.Now, true);
            var anna = new User(AccountType.Regular, LoginMethod.Eventlink, "picUrl", "Anna", "", "Wilson", "Anna Wilson", "anna@eventlink.ml", "address", DateTime.Now, "anna", "00000000", "Denmark",
                new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), DateTime.Now, true);
            var gubba = new User(AccountType.Regular, LoginMethod.Eventlink, "picUrl", "Gubba", "", "", "Gubba", "gubba@eventlink.ml", "address", DateTime.Now, "gubba", "00000000", "Denmark",
                new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), DateTime.Now, true);
            var violet = new User(AccountType.Regular, LoginMethod.Eventlink, "picUrl", "Violet", "G", "Shink", "Violet G Shink", "violet@eventlink.ml", "address", DateTime.Now, "violet", "00000000", "Norway",
                new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), DateTime.Now, true);
            var bean = new User(AccountType.Regular, LoginMethod.Eventlink, "picUrl", "Mr. Bean", "", "", "Mr. Bean", "bean@eventlink.ml", "address", DateTime.Now, "bean", "00000000", "Norway",
                new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(), DateTime.Now, true);

            var a = new User(AccountType.Regular, LoginMethod.Eventlink, "picUrl", "CreateUser", "CreateUser", "CreateUser", "CreateUser", "CreateUser@eventlink.ml", "address", DateTime.Now,
                ConfigurationManager.AppSetting["CreateUser:Password"], "00000000", "",
                new List<string>(), new List<string>(), new List<string>(), new List<string>(), new List<string>(),
                DateTime.Now, true);

            Console.WriteLine("Creating users in database...");

            UserService.CreateUser(daniel);
            UserService.CreateUser(thomas);
            UserService.CreateUser(emma);
            UserService.CreateUser(rose);
            UserService.CreateUser(clayton);
            UserService.CreateUser(meow);
            UserService.CreateUser(stephanov);
            UserService.CreateUser(harold);
            UserService.CreateUser(alicia);
            UserService.CreateUser(howard);
            UserService.CreateUser(jeffrey);
            UserService.CreateUser(john);
            UserService.CreateUser(kenneth);
            UserService.CreateUser(miriam);
            UserService.CreateUser(dennis);
            UserService.CreateUser(anna);
            UserService.CreateUser(gubba);
            UserService.CreateUser(violet);
            UserService.CreateUser(bean);
            UserService.CreateUser(a);

            Console.WriteLine("Getting users...");

            var danielUser = UserService.GetUserByEmail(daniel.Email);
            var thomasUser = UserService.GetUserByEmail(thomas.Email);
            var emmaUser = UserService.GetUserByEmail(emma.Email);
            var roseUser = UserService.GetUserByEmail(rose.Email);
            var claytonUser = UserService.GetUserByEmail(clayton.Email);
            var meowUser = UserService.GetUserByEmail(meow.Email);
            var stephanovUser = UserService.GetUserByEmail(stephanov.Email);
            var haroldUser = UserService.GetUserByEmail(harold.Email);
            var aliciaUser = UserService.GetUserByEmail(alicia.Email);
            var howardUser = UserService.GetUserByEmail(howard.Email);
            var jeffreyUser = UserService.GetUserByEmail(jeffrey.Email);
            var johnUser = UserService.GetUserByEmail(john.Email);
            var kennethUser = UserService.GetUserByEmail(kenneth.Email);
            var miriamUser = UserService.GetUserByEmail(miriam.Email);
            var dennisUser = UserService.GetUserByEmail(dennis.Email);
            var annaUser = UserService.GetUserByEmail(anna.Email);
            var gubbaUser = UserService.GetUserByEmail(gubba.Email);
            var violetUser = UserService.GetUserByEmail(violet.Email);
            var beanUser = UserService.GetUserByEmail(bean.Email);

            var users = new List<User> {danielUser, thomasUser, emmaUser, roseUser, claytonUser, meowUser, stephanovUser, haroldUser, aliciaUser
                , howardUser, jeffreyUser, johnUser, kennethUser, miriamUser, dennisUser, annaUser, gubbaUser, violetUser, beanUser};

            Console.WriteLine("Uploading user profile pictures...");

            UserService.UploadProfilePicture(danielUser.Id, ConfigurationManager.AppSetting["DemoPics:Daniel"]);
            UserService.UploadProfilePicture(thomasUser.Id, ConfigurationManager.AppSetting["DemoPics:Thomas"]);
            UserService.UploadProfilePicture(emmaUser.Id, ConfigurationManager.AppSetting["DemoPics:Emma"]);
            UserService.UploadProfilePicture(roseUser.Id, ConfigurationManager.AppSetting["DemoPics:rose"]);
            UserService.UploadProfilePicture(claytonUser.Id, ConfigurationManager.AppSetting["DemoPics:Clayton"]);
            UserService.UploadProfilePicture(meowUser.Id, ConfigurationManager.AppSetting["DemoPics:Meow"]);
            UserService.UploadProfilePicture(stephanovUser.Id, ConfigurationManager.AppSetting["DemoPics:Stephanov"]);
            UserService.UploadProfilePicture(haroldUser.Id, ConfigurationManager.AppSetting["DemoPics:Harold"]);
            UserService.UploadProfilePicture(aliciaUser.Id, ConfigurationManager.AppSetting["DemoPics:Alicia"]);
            UserService.UploadProfilePicture(howardUser.Id, ConfigurationManager.AppSetting["DemoPics:Howard"]);
            UserService.UploadProfilePicture(jeffreyUser.Id, ConfigurationManager.AppSetting["DemoPics:Jeffrey"]);
            UserService.UploadProfilePicture(johnUser.Id, ConfigurationManager.AppSetting["DemoPics:John"]);
            UserService.UploadProfilePicture(kennethUser.Id, ConfigurationManager.AppSetting["DemoPics:Kenneth"]);
            UserService.UploadProfilePicture(miriamUser.Id, ConfigurationManager.AppSetting["DemoPics:Miriam"]);
            UserService.UploadProfilePicture(dennisUser.Id, ConfigurationManager.AppSetting["DemoPics:Dennis"]);
            UserService.UploadProfilePicture(annaUser.Id, ConfigurationManager.AppSetting["DemoPics:Anna"]);
            UserService.UploadProfilePicture(gubbaUser.Id, ConfigurationManager.AppSetting["DemoPics:Gubba"]);
            UserService.UploadProfilePicture(violetUser.Id, ConfigurationManager.AppSetting["DemoPics:Violet"]);
            UserService.UploadProfilePicture(beanUser.Id, ConfigurationManager.AppSetting["DemoPics:Bean"]);

            for (int i = 0; i < users.Count; i++)
            {
                var u = users[i];
                for (int j = 0; j < users.Count; j++)
                {
                    if (!users[j].Id.Equals(u.Id, StringComparison.OrdinalIgnoreCase))
                    {
                        try
                        {
                            UserService.AddBuddy(u.Id, users[j].Id);
                        }
                        catch (Exception)
                        {

                        }
                    }
                }
            }

            var events = EventService.SearchEvents("", "").ToList();

            if (events.Count <= 0)
            {
                return;
            }

            for (var i = 0; i < 50; i += 2)
            {
                var e = events[i];

                foreach (var user in users)
                {
                    UserService.AddParticipatingEvent(user.Id, e.Id);
                    UserService.AddFavoriteEvent(user.Id, e.Id);
                }
            }

            Console.WriteLine("Done!");
        }

    }

}