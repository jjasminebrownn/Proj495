using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

namespace PhysioDynamik_Prototype_1
{
    public class Patient
    {
        public string Name { get; set; } = string.Empty;
        public int PatientID { get; set; }
        public string Email { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public List<Appointment> AppointmentList { get; set; } = new List<Appointment>();
        private List<string> PaymentMethods { get; set; } = new List<string>();
        public MedicalRecord MedicalRecord { get; set; } = new MedicalRecord();

        // Default constructor
        public Patient()
        {
        }

        // Constructor that accepts 5 arguments
        public Patient(string name, int patientID, string email, string phoneNumber, DateTime dateOfBirth)
        {
            Name = name;
            PatientID = patientID;
            Email = email;
            PhoneNumber = phoneNumber;
            DateOfBirth = dateOfBirth;
        }

        // Method for patient registration
        public static void RegisterPatient(SystemClass system)
        {
            Console.WriteLine("=== Patient Registration ===");

            // Collect patient details
            Console.Write("Enter your First Name: ");
            string firstName = Console.ReadLine() ?? string.Empty;

            Console.Write("Enter your Last Name: ");
            string lastName = Console.ReadLine() ?? string.Empty;

            Console.Write("Enter your Email: ");
            string email = Console.ReadLine() ?? string.Empty;

            Console.Write("Create a Password: ");
            string password = Console.ReadLine() ?? string.Empty;

            Console.Write("Confirm Password: ");
            string confirmPassword = Console.ReadLine() ?? string.Empty;

            Console.Write("Enter your Phone Number: ");
            string phone = Console.ReadLine() ?? string.Empty;

            Console.Write("Enter your Date of Birth (yyyy-mm-dd): ");
            if (!DateTime.TryParse(Console.ReadLine(), out DateTime dob))
            {
                Console.WriteLine("Invalid date format. Try again.");
                return;
            }

            // Validate and create account
            if (Account.CreateAccount(email, password, confirmPassword, firstName, lastName, phone, dob))
            {
                // Add patient to system
                var patient = new Patient(
                    $"{firstName} {lastName}",
                    0, // You might want to generate or get this from a database
                    email,
                    phone,
                    dob
                );

                system.AddPatient(patient);
                Console.WriteLine("Patient account successfully registered!");
            }
        }

        // Method for selecting an appointment
        public void SelectAppointment(SystemClass system, string slot, int therapistID)
        {
            if (system.CheckSlotAvailability(slot))
            {
                var appointment = new Appointment(
                    new Random().Next(1000, 9999),
                    this.PatientID,
                    therapistID,
                    DateTime.Now,
                    "Scheduled"
                );
                system.AddAppointment(appointment, slot);
                AppointmentList.Add(appointment);
            }
            else
            {
                Console.WriteLine("Selected slot is not available.");
            }
        }

        // Fetch a patient by ID
        public static Patient? GetPatientByID(int id)
        {
            Patient? patient = null;
            string connectionString = "Server=localhost;Database=PhysioDynamikDB;Uid=root;Pwd=password;";

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM Patients WHERE PatientID = @PatientID";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PatientID", id);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                patient = new Patient
                                {
                                    PatientID = reader.GetInt32("PatientID"),
                                    Name = reader.GetString("Name"),
                                    Email = reader.GetString("Email"),
                                    PhoneNumber = reader.GetString("PhoneNumber"),
                                    DateOfBirth = reader.GetDateTime("DateOfBirth"),
                                    MedicalRecord = MedicalRecord.GetMedicalRecordByPatientID(id) ?? new MedicalRecord()
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching patient: {ex.Message}");
                }
            }

            return patient;
        }

        // Method to view patient information
        public void ViewPatientInformation()
        {
            Console.WriteLine("Patient Information:");
            Console.WriteLine($"Name: {Name}");
            Console.WriteLine($"Patient ID: {PatientID}");
            Console.WriteLine($"Email: {Email}");
            Console.WriteLine($"Phone Number: {PhoneNumber}");
            Console.WriteLine($"Date of Birth: {DateOfBirth.ToShortDateString()}");
            Console.WriteLine("Appointments: ");
            foreach (var appointment in AppointmentList)
            {
                Console.WriteLine($"  Appointment with Therapist ID {appointment.TherapistID} on {appointment.Time.ToShortDateString()} - Status: {appointment.Status}");
            }
        }

        // Method to upload homework or documents
        public void Upload(string fileName, string fileType)
        {
            Console.WriteLine($"Patient {Name} uploaded {fileName} ({fileType}).");
            SendNotification("Therapist", $"Patient {Name} has uploaded {fileName}.");
        }

        // Method to view resource shared by therapist
        public void ViewResource(string resourceName)
        {
            Console.WriteLine($"Patient {Name} viewed resource: {resourceName}.");
            SendNotification("Therapist", $"Patient {Name} has viewed the resource: {resourceName}.");
        }

        // Method to submit assessment
        public void SubmitAssessment(string assessmentName, string result)
        {
            Console.WriteLine($"Patient {Name} submitted assessment: {assessmentName} with result: {result}.");
            SendNotification("Therapist", $"Patient {Name} has submitted the assessment: {assessmentName}.");
        }

        private void SendNotification(string recipient, string message)
        {
            Console.WriteLine($"Notification sent to {recipient}: {message}");
        }

    }


    public class Therapist
    {
        public int TherapistID { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Specialty { get; set; } = string.Empty;
        public List<Appointment> AppointmentList { get; set; } = new List<Appointment>();
        public bool PaymentConfirmed { get; set; }
        public List<AvailabilitySlot> AvailabilitySlots { get; set; } = new List<AvailabilitySlot>();

        public Therapist(int therapistID, string name, string email, string specialty)
        {
            TherapistID = therapistID;
            Name = name;
            Email = email;
            Specialty = specialty;
        }


        // Method to add an availability slot to a therapist's schedule
        public bool AddAvailability(AvailabilitySlot slot)
        {
            // Check if the time slot is already in the list
            foreach (var existingSlot in AvailabilitySlots)
            {
                if (existingSlot.StartTime == slot.StartTime && existingSlot.EndTime == slot.EndTime)
                {
                    Console.WriteLine($"Failure: The slot from {slot.StartTime} to {slot.EndTime} is already added.");
                    return false;
                }
            }

            AvailabilitySlots.Add(slot);
            Console.WriteLine($"Success: Slot from {slot.StartTime} to {slot.EndTime} added.");
            return true;
        }

        // Method to upload resources or assignments
        public void Upload(string fileName, string fileType)
        {
            Console.WriteLine($"Therapist {Name} uploaded {fileName} ({fileType}).");
            SendNotification("Patient", $"Therapist {Name} has uploaded {fileName}.");
        }

        // Method to provide feedback
        public void ProvideFeedback(string homeworkName, string feedback)
        {
            Console.WriteLine($"Therapist {Name} provided feedback on {homeworkName}: {feedback}.");
            SendNotification("Patient", $"Therapist {Name} has provided feedback on your homework: {homeworkName}.");
        }

        // Method to assign an assessment
        public void AssignAssessment(string assessmentName)
        {
            Console.WriteLine($"Therapist {Name} assigned assessment: {assessmentName}.");
            SendNotification("Patient", $"Therapist {Name} has assigned you an assessment: {assessmentName}.");
        }

        private void SendNotification(string recipient, string message)
        {
            Console.WriteLine($"Notification sent to {recipient}: {message}");
        }

    }

    public class AvailabilitySlot
    {
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        // Constructor for creating a new AvailabilitySlot
        public AvailabilitySlot(DateTime startTime, DateTime endTime)
        {
            StartTime = startTime;
            EndTime = endTime;
        }
    }

    public class Appointment
    {
        public int AppointmentID { get; set; }
        public DateTime Time { get; set; }
        public string Status { get; set; } = string.Empty;
        public int TherapistID { get; set; }
        public int PatientID { get; set; } 
        public DateTime Date { get; set; } 

       public Appointment(int appointmentID, int patientID, int therapistID, DateTime time, string status)
        {
            AppointmentID = appointmentID;
            PatientID = patientID;
            TherapistID = therapistID;
            Time = time;
            Status = status;
        }


        // Schedule an appointment
        public void ScheduleAppointment(DateTime date)
        {
            Date = date;
            Status = "Scheduled";
            Console.WriteLine($"Appointment {AppointmentID} scheduled for {Date}.");
        }

        // Cancel an appointment
        public void CancelAppointment()
        {
            Status = "Cancelled";
            Console.WriteLine($"Appointment {AppointmentID} has been cancelled.");
        }

        // View appointment status
        public void ViewStatus()
        {
            Console.WriteLine($"Appointment {AppointmentID} status: {Status}");
        }
    }

    public class SystemClass
    {
        public int SystemID { get; set; }
        public List<string> AvailableSlots { get; set; } = new List<string>
        {
            "9:00 AM", "10:00 AM", "11:00 AM", "1:00 PM", "2:00 PM"
        };

        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
        public List<Therapist> Therapists { get; set; } = new List<Therapist>();

        // Assuming there's a list of patients
        public List<Patient> Patients { get; set; } = new List<Patient>();

        public void AddPatient(Patient patient)
        {
            if (patient == null)
            {
                Console.WriteLine("Invalid patient data.");
                return;
            }
            
            Patients.Add(patient);
            Console.WriteLine($"Patient {patient.Name} added to the system.");
        }
        

        // Add a therapist to the system
        public void AddTherapist(Therapist therapist)
        {
            if (therapist == null)
            {
                Console.WriteLine("Invalid therapist data.");
                return;
            }
            Therapists.Add(therapist);
            Console.WriteLine($"Success: Therapist {therapist.Name} added to the system.");
        }

        // Upload therapist's availability slots
        public void UploadTherapistAvailabilities(int therapistID, List<AvailabilitySlot> availabilities)
        {
            var therapist = Therapists.Find(t => t.TherapistID == therapistID);
            if (therapist != null)
            {
                foreach (var slot in availabilities)
                {
                    therapist.AddAvailability(slot);
                }
            }
            else
            {
                Console.WriteLine($"Failure: Therapist with ID {therapistID} not found.");
            }
        }

        // Display all therapists and their availability slots
        public void DisplayTherapistAvailabilities()
        {
            foreach (var therapist in Therapists)
            {
                Console.WriteLine($"Therapist: {therapist.Name}");
                Console.WriteLine("Availability Slots:");
                foreach (var slot in therapist.AvailabilitySlots)
                {
                    Console.WriteLine($"- {slot.StartTime} to {slot.EndTime}");
                }
            }
        }

        // Check if a slot is available for a given therapist
        public bool CheckSlotAvailability(int therapistID, string slot)
        {
            var therapist = Therapists.Find(t => t.TherapistID == therapistID);
            if (therapist != null)
            {
                foreach (var availability in therapist.AvailabilitySlots)
                {
                    if (availability.StartTime.ToString("h:mm tt") == slot)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        // Display available slots
        public void DisplayAvailableSlots()
        {
            Console.WriteLine("Available Slots:");
            foreach (var slot in AvailableSlots)
            {
                Console.WriteLine(slot);
            }
        }

        // Check if a slot is available
        public bool CheckSlotAvailability(string slot)
        {
            return AvailableSlots.Contains(slot);
        }

        // Add a new appointment
        public void AddAppointment(Appointment appointment, string slot)
        {
            if (appointment == null)
            {
                Console.WriteLine("Invalid appointment data.");
                return;
            }

            if (CheckSlotAvailability(slot))
            {
                Appointments.Add(appointment);
                AvailableSlots.Remove(slot);
                Console.WriteLine($"Appointment {appointment.AppointmentID} added for slot {slot}.");
            }
            else
            {
                Console.WriteLine("Selected slot is not available.");
            }
        }

        // Retrieve all appointments
        public void GetAppointments()
        {
            Console.WriteLine("Appointments List:");
            foreach (var appointment in Appointments)
            {
                Console.WriteLine($"ID: {appointment.AppointmentID}, Date: {appointment.Date}, Status: {appointment.Status}");
            }
        }

        // Run system to fetch and display patient and medical record details
        public void RunSystem()
        {
            Console.WriteLine("Welcome to PhysioDynamik System");
            Console.WriteLine("Enter Patient ID to view details: ");
            if (!int.TryParse(Console.ReadLine(), out int patientID))
            {
                Console.WriteLine("Invalid Patient ID.");
                return;
            }

            Patient? patient = Patient.GetPatientByID(patientID);

            if (patient != null)
            {
                Console.WriteLine("\n--- Patient Details ---");
                patient.ViewPatientInformation();

                if (patient.MedicalRecord != null)
                {
                    Console.WriteLine("\n--- Medical Record ---");
                    patient.MedicalRecord.ViewMedicalRecord();
                }
                else
                {
                    Console.WriteLine("No medical record found for this patient.");
                }
            }
            else
            {
                Console.WriteLine("Patient not found.");
            }
        }
    }

    public class MedicalRecord
    {
        public int PatientID { get; set; }
        public string Report { get; set; } = string.Empty;
        public List<DateTime> TreatmentHistory { get; set; } = new List<DateTime>();

        public static MedicalRecord? GetMedicalRecordByPatientID(int patientID)
        {
            MedicalRecord? record = null;
            string connectionString = "Server=localhost;Database=PhysioDynamikDB;Uid=root;Pwd=password;";

            using (var connection = new MySqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    string query = "SELECT * FROM MedicalRecords WHERE PatientID = @PatientID";
                    using (var cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@PatientID", patientID);
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                record = new MedicalRecord
                                {
                                    PatientID = reader.GetInt32("PatientID"),
                                    Report = reader.GetString("Report"),
                                    TreatmentHistory = new List<DateTime>
                                    {
                                        DateTime.Now.AddDays(-10),
                                        DateTime.Now.AddDays(-5)
                                    }
                                };
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error fetching medical record: {ex.Message}");
                }
            }

            return record;
        }

        public void ViewMedicalRecord()
        {
            Console.WriteLine($"Report: {Report}");
            Console.WriteLine("Treatment History:");
            foreach (var date in TreatmentHistory)
            {
                Console.WriteLine($"- {date.ToShortDateString()}");
            }
        }
    }


    public class Assessment
    {
        private int ClientID { get; set; }
        private string Description { get; set; } = string.Empty;
        private string Status { get; set; } = string.Empty;
        private string Result { get; set; } = string.Empty;

        // Constructor
        public Assessment(int clientId, string description = "", string status = "", string result = "")
        {
            ClientID = clientId;
            Description = description;
            Status = status;
            Result = result;
        }
    }

    public class ConfirmationEmail
    {
        // Attributes
        public int PatientID { get; set; } // The ID of the patient
        public string Email { get; set; } = string.Empty; // The email address to send the confirmation to
        public string Message { get; set; } = string.Empty; // The confirmation message

        // Constructor
        public ConfirmationEmail(int patientID, string email, string message = "")
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                throw new ArgumentException("Email cannot be null or empty", nameof(email));
            }
            PatientID = patientID;
            Email = email;
            Message = message;
        }

        // Method: Send Confirmation Email
        public void Send()
        {
            if (string.IsNullOrWhiteSpace(Email))
            {
                Console.WriteLine("Invalid email address. Cannot send confirmation email.");
                return;
            }

            // Simulate sending an email
            Console.WriteLine($"Sending confirmation email to {Email}...");
            Console.WriteLine($"Patient ID: {PatientID}");
            Console.WriteLine($"Message: {Message}");
            Console.WriteLine("Confirmation email sent successfully!");
        }
    }
    public class RegistrationForm
    {
        // Attributes
        public int FormID { get; set; } // Unique identifier for the form
        public Dictionary<string, string> PatientDetails { get; set; } = new Dictionary<string, string>(); // Stores patient information as key-value pairs

        // Constructor
        public RegistrationForm(int formID)
        {
            FormID = formID;
        }

        // Method: Add Patient Details
        public void AddPatientDetail(string key, string value)
        {
            if (string.IsNullOrWhiteSpace(key) || string.IsNullOrWhiteSpace(value))
            {
                Console.WriteLine("Key and value cannot be empty.");
                return;
            }

            if (PatientDetails.ContainsKey(key))
            {
                Console.WriteLine($"Key '{key}' already exists. Updating value...");
                PatientDetails[key] = value; // Update the existing value
            }
            else
            {
                PatientDetails.Add(key, value); // Add new key-value pair
            }
        }

        // Method: Validate Information
        public bool ValidateInformation()
        {
            string[] requiredFields = { "Name", "Email", "PhoneNumber", "DateOfBirth" };

            foreach (var field in requiredFields)
            {
                if (!PatientDetails.ContainsKey(field) || string.IsNullOrWhiteSpace(PatientDetails[field]))
                {
                    Console.WriteLine($"Missing or invalid information for: {field}");
                    return false;
                }
            }

            Console.WriteLine("All patient information is valid.");
            return true;
        }

        // Method: Display Form Details
        public void DisplayFormDetails()
        {
            Console.WriteLine($"Registration Form ID: {FormID}");
            foreach (var detail in PatientDetails)
            {
                Console.WriteLine($"{detail.Key}: {detail.Value}");
            }
        }
    }
    public class Account
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }

        // Static method to create an account
        private static bool ValidateAccountDetails(string email, string password, string confirmPassword, string firstName, string lastName, string phone, DateTime dob, out List<string> errors)
        {
            errors = new List<string>();

            if (string.IsNullOrWhiteSpace(email)) errors.Add("Email cannot be empty.");
            if (string.IsNullOrWhiteSpace(password)) errors.Add("Password cannot be empty.");
            if (password != confirmPassword) errors.Add("Passwords do not match.");
            if (string.IsNullOrWhiteSpace(firstName)) errors.Add("First name cannot be empty.");
            if (string.IsNullOrWhiteSpace(lastName)) errors.Add("Last name cannot be empty.");
            if (string.IsNullOrWhiteSpace(phone)) errors.Add("Phone number cannot be empty.");
            if (dob == DateTime.MinValue) errors.Add("Invalid date of birth.");

            return errors.Count == 0;
        }

        public static bool CreateAccount(string email, string password, string confirmPassword, string firstName, string lastName, string phone, DateTime dob)
        {
            if (ValidateAccountDetails(email, password, confirmPassword, firstName, lastName, phone, dob, out List<string> errors))
            {
                Console.WriteLine("Account created successfully.");
                return true;
            }

            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }
            return false;
        }
    }

}
