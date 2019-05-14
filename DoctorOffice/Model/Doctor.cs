using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace DoctorOffice.Models
{
  public class Doctor
  {
    private string _name;
    private int _id;

    public Doctor(string doctorName, int id = 0)
    {
      _name = doctorName;
      _id = id;
    }

    public string GetName()
    {
      return _name;
    }

    public void SetName(string newName)
    {
      _name = newName;
    }

    public int GetId()
    {
      return _id;
    }

    public static List<Doctor> GetAll()
    {
      List<Doctor> allDoctors = new List<Doctor> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM doctors;";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int DoctorId = rdr.GetInt32(0);
        string DoctorName = rdr.GetString(1);
        Doctor newDoctor = new Doctor(DoctorName, DoctorId);
        allDoctors.Add(newDoctor);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allDoctors;
    }

    public static void ClearAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM doctors;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static Doctor Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = new MySqlCommand(@"SELECT * FROM doctors WHERE id = (@searchId);", conn);
      cmd.Parameters.AddWithValue("@searchId", id);
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int DoctorId = 0;
      string DoctorName = "";
      // We remove the line setting a patientDoctorId value here.
      while(rdr.Read())
      {
        DoctorId = rdr.GetInt32(0);
        DoctorName = rdr.GetString(1);
        // We no longer read the patientDoctorId here, either.
      }
      // Constructor below no longer includes a patientDoctorId parameter:
      Doctor newDoctor = new Doctor(DoctorName, DoctorId);
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return newDoctor;
    }

    public List<Patient> GetPatients()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      MySqlCommand cmd = new MySqlCommand(@"SELECT patient_id FROM doctors_patients WHERE doctor_id = @DoctorId;", conn);
      cmd.Parameters.AddWithValue("@DoctorId", _id);
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      List<int> patientIds = new List<int> {};
      while(rdr.Read())
      {
        int patientId = rdr.GetInt32(0);
        patientIds.Add(patientId);
      }
      rdr.Dispose();
      List<Patient> patients = new List<Patient> {};
      foreach (int patientId in patientIds)
      {
        var patientQuery = conn.CreateCommand() as MySqlCommand;
        patientQuery.CommandText = @"SELECT * FROM patients WHERE id = @PatientId;";
        MySqlParameter patientIdParameter = new MySqlParameter();
        patientIdParameter.ParameterName = "@PatientId";
        patientIdParameter.Value = patientId;
        patientQuery.Parameters.Add(patientIdParameter);
        var patientQueryRdr = patientQuery.ExecuteReader() as MySqlDataReader;
        while(patientQueryRdr.Read())
        {
          int thisPatientId = patientQueryRdr.GetInt32(0);
          string patientDescription = patientQueryRdr.GetString(1);
          Patient foundPatient = new Patient(patientDescription, thisPatientId);
          patients.Add(foundPatient);
        }
        patientQueryRdr.Dispose();
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return patients;
    }

    public override bool Equals(System.Object otherDoctor)
    {
      if (!(otherDoctor is Doctor))
      {
        return false;
      }
      else
      {
        Doctor newDoctor = (Doctor) otherDoctor;
        bool idEquality = this.GetId().Equals(newDoctor.GetId());
        bool nameEquality = this.GetName().Equals(newDoctor.GetName());
        return (idEquality && nameEquality);
      }
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = new MySqlCommand(@"INSERT INTO doctors (name) VALUES (@name);", conn);
      cmd.Parameters.AddWithValue("@name", this._name);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId; // <-- This line is new!
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void DeleteDoc(int doctorId)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;

      Doctor selectedDoctor = Doctor.Find(doctorId);
      Dictionary<string, object> model = new Dictionary<string, object>();
      List<Patient> doctorPatient = selectedDoctor.GetPatients();
      model.Add("doctor", selectedDoctor);

      foreach (Patient patient in doctorPatient)
      {
        patient.Delete();
      }

      cmd.CommandText = @"DELETE FROM doctors WHERE id = @thisId;";
      cmd.Parameters.AddWithValue("@thisId", _id);
      cmd.ExecuteNonQuery();

      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void Delete()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = new MySqlCommand("DELETE FROM doctors WHERE id = @DoctorId; DELETE FROM doctors_patients WHERE doctor_id = @DoctorId;", conn);
      cmd.Parameters.AddWithValue("@DoctorId", this.GetId());
      cmd.ExecuteNonQuery();
      if (conn != null)
      {
        conn.Close();
      }
    }

    public void AddPatient(Patient newPatient)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = new MySqlCommand(@"INSERT INTO doctors_patients (doctor_id, patient_id) VALUES (@DoctorId, @PatientId);", conn);
      cmd.Parameters.AddWithValue("@DoctorId", _id);
      cmd.Parameters.AddWithValue("@PatientId", newPatient.GetId());
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }


    }
  }
