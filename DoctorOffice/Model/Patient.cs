using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace DoctorOffice.Models
{
  public class Patient
  {
    private string _name;
    private int _id;

    public Patient(string patientName, int id = 0)
    {
      _name = patientName;
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

    public static List<Patient> GetAll()
    {
      List<Patient> allPatients = new List<Patient> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM patients;";
      MySqlDataReader rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int patientId = rdr.GetInt32(0);
        string patientName = rdr.GetString(1);

        Patient newPatient = new Patient(patientName, patientId);
        allPatients.Add(newPatient);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allPatients;
    }

    public static void ClearAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM patients;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static Patient Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = new MySqlCommand(@"SELECT * FROM patients WHERE id = (@searchId);", conn);
      cmd.Parameters.AddWithValue("@searchId", id);
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int patientId = 0;
      string patientName = "";
      // We remove the line setting a itemCategoryId value here.
      while(rdr.Read())
      {
        patientId = rdr.GetInt32(0);
        patientName = rdr.GetString(1);
        // We no longer read the itemCategoryId here, either.
      }
      // Constructor below no longer includes a itemCategoryId parameter:
      Patient newPatient = new Patient(patientName, patientId);
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return newPatient;
    }

    public override bool Equals(System.Object otherPatient)
    {
      if (!(otherPatient is Patient))
      {
        return false;
      }
      else
      {
        Patient newPatient = (Patient) otherPatient;
        bool idEquality = this.GetId() == newPatient.GetId();
        bool nameEquality = this.GetName() == newPatient.GetName();
        // We no longer compare Patients' doctorIds here.
        return (idEquality && nameEquality);
      }
    }

    public void Save()
    {
      // Code to declare, set, and add values to a doctorId SQL parameters has also been removed.
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = new MySqlCommand(@"INSERT INTO patients (name) VALUES (@name);", conn);
      cmd.Parameters.AddWithValue("@name", this._name);
      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void Edit(string newName)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = new MySqlCommand(@"UPDATE patients SET name = @newName WHERE id = @searchId;", conn);
      cmd.Parameters.AddWithValue("@searchId", _id);
      cmd.Parameters.AddWithValue("@newName", newName);
      cmd.ExecuteNonQuery();
      _name = newName;
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public List<Doctor> GetDoctors()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = new MySqlCommand(@"SELECT doctor_id FROM doctors_patients WHERE patient_id = @patientId;", conn);
      cmd.Parameters.AddWithValue("@patientId", _id);
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      List<int> doctorIds = new List<int> {};
      while(rdr.Read())
      {
        int doctorId = rdr.GetInt32(0);
        doctorIds.Add(doctorId);
      }
      rdr.Dispose();
      List<Doctor> doctors = new List<Doctor> {};
      foreach (int doctorId in doctorIds)
      {
        MySqlCommand command = new MySqlCommand(@"SELECT * FROM doctors WHERE id = @DoctorId;", conn);
        command.Parameters.AddWithValue("@DoctorId", doctorId);
        var doctorQueryRdr = command.ExecuteReader() as MySqlDataReader;
        while(doctorQueryRdr.Read())
        {
          int thisDoctorId = doctorQueryRdr.GetInt32(0);
          string doctorName = doctorQueryRdr.GetString(1);
          Doctor foundDoctor = new Doctor(doctorName, thisDoctorId);
          doctors.Add(foundDoctor);
        }
        doctorQueryRdr.Dispose();
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return doctors;
    }

    public void AddDoctor(Doctor newDoctor)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = new MySqlCommand(@"INSERT INTO doctors_patients (doctor_id, patient_id) VALUES (@DoctorId, @PatientId);", conn);
      cmd.Parameters.AddWithValue("@DoctorId", newDoctor.GetId());
      cmd.Parameters.AddWithValue("@PatientId", _id);
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
      MySqlCommand cmd = new MySqlCommand(@"DELETE FROM patients WHERE id = @PatientId; DELETE FROM categories_patients WHERE patient_id = @PatientId;", conn);
      cmd.Parameters.AddWithValue("@PatientId", this.GetId());
      cmd.ExecuteNonQuery();
      if (conn != null)
      {
        conn.Close();
      }
    }

  }
}
