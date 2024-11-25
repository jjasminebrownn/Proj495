using System;
using MySql.Data.MySqlClient; // Import MySQL library
using PhysioDynamik_Prototype_1;

namespace PhysioDynamik_Prototype_1
{
    class Program
    {
        static void Main(string[] args)
        {
            // Initialize the system and patients
            SystemClass system = new SystemClass();
            Patient patient = new Patient
            {
                Name = "John Doe",
                PatientID = 1,
                Email = "john.doe@example.com",
                PhoneNumber = "123-456-7890",
                DateOfBirth = new DateTime(1990, 1, 1)
            };

            // 1. Register a New Patient
            Console.WriteLine("\n---- Register a New Patient ----");
            Patient.RegisterPatient(system);

            // 2. Add a Therapist to the System and Upload Availability
            Console.WriteLine("\n---- Add a Therapist and Upload Availability ----");
            Therapist therapist = new Therapist(101, "Dr. Smith", "dr.smith@example.com", "Physiotherapy");
            system.AddTherapist(therapist);
            therapist.AddAvailability(new AvailabilitySlot(DateTime.Now.AddHours(1), DateTime.Now.AddHours(2)));

            // 3. Display Available Slots
            Console.WriteLine("\n---- Display Available Slots ----");
            system.DisplayAvailableSlots();

            // 4. Book an Appointment for a Therapist's Slot
            Console.WriteLine("\n---- Book an Appointment ----");
            patient.SelectAppointment(system, "10:00 AM", 101); // Assuming TherapistID = 101

            // 5. View All Appointments of the Patient and Therapist
            Console.WriteLine("\n---- View Patient Appointments ----");
            patient.ViewPatientInformation();

            Console.WriteLine("\n---- View Therapist Appointments ----");
            foreach (var appointment in therapist.AppointmentList)
            {
                appointment.ViewStatus();
            }

            // 6. Cancel an Appointment
            Console.WriteLine("\n---- Cancel an Appointment ----");
            if (patient.AppointmentList.Count > 0)
            {
                Appointment appointmentToCancel = patient.AppointmentList[0];
                appointmentToCancel.CancelAppointment();
            }

            // 7. Display Updated Available Slots After Cancellation
            Console.WriteLine("\n---- Display Updated Available Slots ----");
            system.DisplayAvailableSlots();

            // 8. Add Multiple Availability Slots for a Therapist
            Console.WriteLine("\n---- Add Multiple Availability Slots for Therapist ----");
            var newSlots = new List<AvailabilitySlot>
            {
                new AvailabilitySlot(DateTime.Now.AddHours(3), DateTime.Now.AddHours(4)),
                new AvailabilitySlot(DateTime.Now.AddHours(5), DateTime.Now.AddHours(6))
            };
            system.UploadTherapistAvailabilities(101, newSlots);

            // 9. Assign and Submit an Assessment
            Console.WriteLine("\n---- Assign and Submit Assessment ----");
            therapist.AssignAssessment("Back Pain Assessment");
            patient.SubmitAssessment("Back Pain Assessment", "Passed");

            // 10. Upload and View Resources
            Console.WriteLine("\n---- Upload and View Resources ----");
            therapist.Upload("Stretching Guide", "PDF");
            patient.ViewResource("Stretching Guide");

            // 11. Display Patient's Medical Record
            Console.WriteLine("\n---- Display Patient's Medical Record ----");
            if (patient.MedicalRecord != null)
            {
                patient.MedicalRecord.ViewMedicalRecord();
            }
            else
            {
                Console.WriteLine("No medical record available for this patient.");
            }

            // 12. Display Updated Appointment Status (Post Cancellation)
            Console.WriteLine("\n---- View Updated Appointment Status ----");
            if (patient.AppointmentList.Count > 0)
            {
                patient.AppointmentList[0].ViewStatus();
            }

            // Final Test: Display Available Slots After All Tests
            Console.WriteLine("\n---- Display Available Slots After All Tests ----");
            system.DisplayAvailableSlots();
        }
    }
}
