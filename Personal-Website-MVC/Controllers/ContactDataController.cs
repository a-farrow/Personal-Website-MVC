using Microsoft.AspNetCore.Mvc;
using Personal_Website_MVC.Models;
using Microsoft.Extensions.Options; //Grants access to IOptions<T>
using MimeKit; //Added for access to MimeMessage class
using MailKit.Net.Smtp; //Added for access to SmtpClient class
//RECAPTCHA - STEP 08
// - Add the using statement below
using PaulMiami.AspNetCore.Mvc.Recaptcha;//added for access to reCAPTCHA annotation
using Microsoft.Data.SqlClient;
using System.Configuration;//Added for access to Connected SQL classes

namespace CORE1.Controllers
{
    public class ContactDataController : Controller
    {
        //We won't be using an Index Action/View for this controller,
        //so we can simply comment out (or delete) the one that is provided.
        //public IActionResult Index()
        //{
        //    return View();
        //}

        #region Adding Credentials to appsettings.json

        //Before creating any Actions or Views related to this Controller,
        //we will add a Credentials section to the appsettings.json file.
        //This lets us store the (sensitive) login information for our 
        //email account in that file so it does not have to be written here.

        //If you are using source control, you can then add the following line
        //to your .gitignore file to prevent the appsettings.json from being
        //pushed to the remote repo:

        // */appsettings.json

        #endregion

        //Create a field to store the CredentialSettings info
        private readonly CredentialSettings _credentials;

        // below is a field to store our connection string
        private readonly string _connectionString;

        //Add a constructor for our Controller that accepts the neccessary info as parameters
        public ContactDataController(IOptions<CredentialSettings> settings, IConfiguration config)
        {
            //Since settings is of type IOptions<CredentialsSettings>, we will need to
            //access the Value property to get the info we want from CredentialSettings
            _credentials = settings.Value;

            //We can retrieve a connection string from appsettings.json by utilizing the
            //IConfiguration parameter, which has a method for retrieving the connection strings:
            _connectionString = config.GetConnectionString("NorthwindConnection");
        }

        //MINI-LAB!
        //In your PersonalSite solution, open the HomeController and
        //add the above using statement and field.
        //Next, update the constructor currently in your HomeController
        //to accept an additional IOptions<CredentialSettings> parameter and assign it

        //Controller Actions are meant to handle certain types of requests. The most
        //common request is GET, which is used to request info to load a page. We will
        //also create actions to handle POST requests, which are used to send info to the app.

        //GET is the default request type to be handled, so no extra info is needed here.
        public IActionResult Contact()
        {
            return View();

            //We want the info from our Contact form to use the ContactViewModel we created.
            //To do this, we can generate the necessary code using the following steps:

            #region Code Generation Steps

            //1. Go to Tools > NuGet Package Manager > Manage NuGet Packages for Solution
            //2. Go to the Browse tab and search for Microsoft.VisualStudio.Web
            //3. Click Microsoft.VisualStudio.Web.CodeGeneration.Design
            //4. On the right, check the box next to the CORE1 project, then click "Install"
            //5. Once installed, return here and right click the Contact action
            //6. Select Add View, then select the Razor View template and click "Add"
            //7. Enter the following settings:
            //      - View Name: Contact
            //      - Template: Create
            //      - Model Class: ContactViewModel
            //8. Leave all other settings as-is and click "Add"

            #endregion
        }

        //Now we need to handle what to do when the user submits the form. For this,
        //we will make another Contact action, this time intended to handle the POST request.
        [HttpPost]
        //RECAPTCHA - STEP 09
        // - Add ValidateRecaptcha annotation below
        
        public IActionResult Contact(ContactViewModel cvm)
        {
            //RECAPTCHA - STEP 10
            //Make a ViewBag property for any potential reCAPTCHA error:
            ViewBag.RecaptchaError = null;

            //When a class has validation attributes, that validation should be checked
            //BEFORE attempting to process any of the data they provided.

            if (!ModelState.IsValid)
            {
                //RECAPTCHA - STEP 11
                // - Assign error message to ViewBag
                ViewBag.RecaptchaError = "Please confirm you are not a robot before sending a message.";

                //Send them back to the form. We can pass the boject to the view
                //so the form will contain the original information they provided.

                return View(cvm);
            }

            //To handle sending the email, we'll need to install a NuGet Package
            //and add a few using statements. We can do this with the following steps:

            #region Email Setup Steps & Email Info

            //1. Go to Tools > NuGet Package Manager > Manage NuGet Packages for Solution
            //2. Go to the Browse tab and search for NETCore.MailKit
            //3. Click NETCore.MailKit
            //4. On the right, check the box next to the CORE1 project, then click "Install"
            //5. Once installed, return here
            //6. Add the following using statements & comments:
            //      - using MimeKit; //Added for access to MimeMessage class
            //      - using MailKit.Net.Smtp; //Added for access to SmtpClient class
            //7. Once added, return here to continue coding email functionality

            //MIME - Multipurpose Internet Mail Extensions - Allows email to contain
            //information other than ASCII, including audio, video, images, and HTML

            //SMTP - Simple Mail Transfer Protocol - An internet protocol (similar to HTTP)
            //that specializes in the collection & transfer of email data
            #endregion

            //Create the format for the message content we will receive from the contact form
            string message = $"You have received a new email from your site's contact form!<br />" +
                $"Sender: {cvm.Name}<br />Email: {cvm.Email}<br />Subject: {cvm.Subject}<br />" +
                $"Message: {cvm.Message}";

            //Create a MimeMessage object to assist with storing/transporting the
            //email information from the contact form.
            var mm = new MimeMessage();

            //Even though the user is the one attemting to send a message to us, the actual sender
            //of the email is the email user we set up with our hosting provider.

            //We can access the credentials for this email user from our appsettings.json file as shown below:
            mm.From.Add(new MailboxAddress("Sender", _credentials.Email.Username));

            //The recipient of this email will be our personal email address, also stored in appsettings.json
            mm.To.Add(new MailboxAddress("Personal", _credentials.Email.Recipient));

            //The subject will be the one provided by the user, which we stored in our cvm object
            mm.Subject = $"New contact form message: [{cvm.Subject}]";

            //The body of the message will be formatted with the string we created above
            mm.Body = new TextPart("HTML") { Text = message };

            //We can set the priority of the message as "urgent" so it will be flagged in our email client.
            mm.Priority = MessagePriority.Urgent;

            //We can also add the user's provided email address to the list of ReplyTo addresses
            //so our replies can be sent directly to them instead of the email user on our hosting provider.
            mm.ReplyTo.Add(new MailboxAddress("User", cvm.Email));

            //The using directive will create the SmptClient object used to send the email.
            //Once all of the code inside of the using directive's scope has been executed,
            //it will close any open connections and dispose of the object for us.
            using (var client = new SmtpClient())
            {
                //Connect to the mail server using credentials in our appsettings.json & port 8889
                client.Connect(_credentials.Email.Server, 8889);

                //Log in to the mail server using the credentials for our email user
                client.Authenticate(_credentials.Email.Username, _credentials.Email.Password);

                //It's possible the mail server may be down when the user attempts to contact us,
                //so we can "encapsulate" our code to send the message in a try/catch
                try
                {
                    //Try to send the email:
                    client.Send(mm);
                }
                catch (Exception ex)
                {
                    //If there is an issue, we can store an error message in a ViewBag variable
                    //to be displayed in the View.
                    ViewBag.ErrorMessage = $"There was an error in processing your request." +
                        $"Please try again later.<br />Error Message: {ex.StackTrace}";

                    //Return the user to the View with ther form information intact.
                    return View(cvm);
                }
            }

            //If all goes well, return a VIew that displays a confirmation to the user
            //that their email was sent.
            return View("EmailConfirmation", cvm);
        }

        //DomainModels - GET
        public IActionResult DomainModels()
        {
            #region Connected SQL Notes

            //Connected SQL allows us to connect our application to a SQL
            //database to create, read, update, and delete (CRUD) data. 
            //More specifically, Connected SQL refers to the process of
            //sending a request to the database every time we want to 
            //execute a query.

            //An alternative to this approach is Disconnected SQL, in which
            //all requests would be stored in a "batch" and executed all
            //at once, typically in the off-hours of the business. 

            //In this application, we will only be working with Connected SQL.
            //To use it, we will need to follow these steps:
            //1. Install Microsoft.Data.SqlClient via the NuGet Package Manager
            //2. Add a using statement for Microsoft.Data.SqlClient
            //3. Define a Connection String
            //4. Create a SqlConnection object (which uses the Connection String)
            //5. Create a SqlCommand object (which uses the SqlConnection object & a query)
            //6. Create a SqlDataReader object to execute the query
            //7. Return the results to the client for processing
            #endregion

            //In order to connect to a database, we will need to provide a connection
            //string. We could write it out here, but it's more efficient to write it 
            //once and store it somewhere so we can access it when needed. We will
            //add our connection string to the appsettings.json file, which has the
            //added benefit of being hidden from source control if we choose.
            //(We added a field called _connectionString at the top of this file,
            //then fetched the value from appsettings.json in this controller's constructor)

            //We intend to retrieve Categories records from the Northwind database.
            //In order to interpret this data in C#, we'll need to create a model.

            //Create a List to store the categories data returned from the database
            List<CategoryDomainModel> categories = new List<CategoryDomainModel>();

            //Using directives allow us to automatically "dispose" of any objects
            //used in a scope once the end of that scope is reaches. Not only does
            //this free up memory, but it also closes the connection to the database,
            //which is useful in preventing issues with frequent requests to the DB.
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                //Create a SqlCommand object with the query & connection we want to use
                SqlCommand cmd = new SqlCommand(
                    "SELECT CategoryId, CategoryName, Description FROM Categories;", conn);

                //Open the connection to the database
                conn.Open();

                //Create a SqlDataReader object to execute the query and read the results
                SqlDataReader reader = cmd.ExecuteReader();

                //Use a while loop to perform a set of actions as long as there is data to read
                while (reader.Read())
                {
                    //For each row of data we receive, create a new CategoryDomainModel object
                    CategoryDomainModel c = new CategoryDomainModel()
                    {
                        //Assign the values in the row of data to their respective properties.
                        //Use explicit casting to ensure the data is stored as the correct type.
                        CategoryID = (int)reader["CategoryId"],
                        CategoryName = (string)reader["CategoryName"],
                        //Any nullable fields should be checked to see if they are null.
                        //If so, reader will see it as a DBNull, which we will need to
                        //store as a C# null in our CategoryDomainModel object.
                        Description = reader["Description"] is DBNull ? null : (string)reader["Description"]
                    };
                    //Add the completed CategoryDomainModel object to the list
                    categories.Add(c);
                }

                //Now that the data has been retrieved, close the SqlDataReader
                reader.Close();
            }
            //Once we arrive at this scope, the SqlConnection will be disposed of automatically

            //Return the View and pass in the List of Categories to be displayed
            return View(categories);

        }

        [HttpPost]
        public IActionResult AddCategory(string categoryName, string categoryDescription)
        {
            //We added parameters to this Action for the data we expect to receive from the form

            //Create a new CategoryDomainModel object:
            CategoryDomainModel category = new CategoryDomainModel()
            {
                //Assing the parameter values to this object's properties.
                //CategoryID is not required since it is the primary key (auto-assigned).
                CategoryName = categoryName,
                Description = categoryDescription
                //Picture is not required since it will not be in the INSERT statement.
            };

            //Create a SqlConnection object with the connection string above
            using (SqlConnection conn = new SqlConnection(_connectionString))
            {
                //Create the SqlCommand object
                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO Categories (CategoryName, Description) " +
                    "VALUES (@CategoryName, @Description)", conn);

                //SQL Parameters should be used to provide dynamic content in the query
                cmd.Parameters.AddWithValue("@CategoryName", category.CategoryName);

                //Handle any possible null values -- translate from C# null to DBNull
                if (categoryDescription == null)
                {
                    cmd.Parameters.AddWithValue("@Description", DBNull.Value);
                }
                else
                {
                    cmd.Parameters.AddWithValue("@Description", category.Description);
                }

                //Open the connection
                conn.Open();

                //Execute the command - ExecuteNonQuery is used for INSERTs (and most CRUD)
                cmd.ExecuteNonQuery();
            }//Automatically disposes of the Connection obj

            //Redirect the user to the DomainModels View to see the updated Categories
            return RedirectToAction("DomainModels");
        }

        public IActionResult EntityModels()
        {
            return View();
        }
    }
}
