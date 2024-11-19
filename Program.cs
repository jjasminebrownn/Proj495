using System;
using MySql.Data.MySqlClient; // Import MySQL library
using PhysioDynamik_Prototype_1;
using System;
using System.Collections.Generic;

public class MedicalRecord
{
    public int PatientID { get; private set; }
    public string? Report { get; set; }
    public List<DateTime> TreatmentHistory { get; set; }

    // Constructor that accepts a patient ID
    public MedicalRecord(int patientID)
    {
        PatientID = patientID;
        TreatmentHistory = new List<DateTime>();
    }
}

public class Patient
{
    public string Name { get; set; }
    public int PatientID { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public DateTime DateOfBirth { get; set; }
    public List<Appointment> AppointmentList { get; set; }
    private List<string> PaymentMethods { get; set; }
    private MedicalRecord MedicalRecord { get; set; }

    public Patient(string name, int patientID, string email, string phoneNumber, DateTime dateOfBirth)
    {
        Name = name;
        PatientID = patientID;
        Email = email;
        PhoneNumber = phoneNumber;
        DateOfBirth = dateOfBirth;
        AppointmentList = new List<Appointment>();
        PaymentMethods = new List<string>();
        MedicalRecord = new MedicalRecord(patientID);
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

    // Method to send notifications
    public void SendNotification(string recipientType, string message)
    {
        Console.WriteLine($"Notification sent to {recipientType}: {message}");
    }

    // Expose medical record for use
    public MedicalRecord GetMedicalRecord()
    {
        return MedicalRecord;
    }

    // Method to access payment methods safely
    public List<string> GetPaymentMethods()
    {
        return PaymentMethods;
    }
}

public class Therapist
{
    public int TherapistID { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    public string Specialty { get; set; }
    public List<Appointment> AppointmentList { get; set; }
    public bool PaymentConfirmed { get; set; }

    public Therapist(int therapistID, string name, string email, string specialty)
    {
        TherapistID = therapistID;
        Name = name;
        Email = email;
        Specialty = specialty;
        AppointmentList = new List<Appointment>();
    }

    // Method to upload resources or assignments
    public void Upload(string fileName, string fileType)
    {
        Console.WriteLine($"Therapist {Name} uploaded {fileName} ({fileType}).");
        SendNotification("Patient", $"Therapist {Name} has uploaded {fileName}.");
    }

    // Method to send notifications
    public void SendNotification(string recipientType, string message)
    {
        Console.WriteLine($"Notification sent to {recipientType}: {message}");
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
}

public class Appointment
{
    public int AppointmentID { get; set; }
    public int PatientID { get; set; }
    public int TherapistID { get; set; }
    public DateTime Time { get; set; }
    public string Status { get; set; } // e.g., "Scheduled", "Cancelled"

    public Appointment(int appointmentID, int patientID, int therapistID, DateTime time, string status)
    {
        AppointmentID = appointmentID;
        PatientID = patientID;
        TherapistID = therapistID;
        Time = time;
        Status = status;
    }
}

public class Homework
{
    public int HomeworkID { get; set; }
    public int patientID { get; set; }
    public int TherapistID { get; set; }
    public DateTime DueDate { get; set; }
    public string SubmissionStatus { get; set; }

    public Homework(int homeworkID, int clientID, int therapistID, DateTime dueDate, string submissionStatus)
    {
        HomeworkID = homeworkID;
        patientID = clientID;
        TherapistID = therapistID;
        DueDate = dueDate;
        SubmissionStatus = submissionStatus;
    }
}

// Main entry point for the application
public class Program
{
    public static void Main(string[] args)
    {
        // Create a new patient instance
        Patient patient = new Patient("John Doe", 101, "johndoe@example.com", "555-1234", new DateTime(1985, 5, 15));

        // Create a therapist and an appointment for the patient
        Therapist therapist = new Therapist(201, "Dr. Smith", "drsmith@example.com", "Psychologist");
        Appointment appointment = new Appointment(301, patient.PatientID, therapist.TherapistID, new DateTime(2024, 11, 20, 10, 0, 0), "Scheduled");
        patient.AppointmentList.Add(appointment);

        // View patient information
        patient.ViewPatientInformation();
    }
}
