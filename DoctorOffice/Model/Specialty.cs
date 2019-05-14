using System.Collections.Generic;
using MySql.Data.MySqlClient;
using System;

namespace DoctorOffice.Models
{
  public class Specialty
  {
    private string _specialty;
    private int _id;

    public Specialty (string specialtyName, int id = 0)
    {
      _specialty = specialtyName;
      _id = id;
    }

    public string GetName()
    {
      return _specialty;
    }

    public void SetName(string newSpecialty)
    {
      _specialty = newSpecialty;
    }

    public int GetId()
    {
      return _id;
    }

    public static List<Specialty> GetAll()
    {
      List<Specialty> allSpecialties = new List<Specialty> {};
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"SELECT * FROM specialties;";
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      while(rdr.Read())
      {
        int SpecialtyId = rdr.GetInt32(0);
        string SpecialtyName = rdr.GetString(1);
        Specialty newSpecialty = new Specialty(SpecialtyName, SpecialtyId);
        allSpecialties.Add(newSpecialty);
      }
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return allSpecialties;
    }

    public static void ClearAll()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;
      cmd.CommandText = @"DELETE FROM specialties;";
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public static Specialty Find(int id)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = new MySqlCommand(@"SELECT * FROM specialties WHERE id = (@searchId);", conn);
      cmd.Parameters.AddWithValue("@searchId", id);
      var rdr = cmd.ExecuteReader() as MySqlDataReader;
      int SpecialtyId = 0;
      string SpecialtyName = "";
      while(rdr.Read())
      {
        SpecialtyId = rdr.GetInt32(0);
        SpecialtyName = rdr.GetString(1);

      }

      Specialty newSpecialty = new Specialty(SpecialtyName, SpecialtyId);
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
      return newSpecialty;
    }

    public List<Doctor> GetDoctors()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();

      MySqlCommand cmd = new MySqlCommand(@"SELECT doctor_id FROM doctors_specialties WHERE specialty_id = @SpecialtyId;", conn);
      cmd.Parameters.AddWithValue("@SpecialtyId", _id);
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
        var doctorQuery = conn.CreateCommand() as MySqlCommand;
        doctorQuery.CommandText = @"SELECT * FROM doctors WHERE id = @DoctorId;";
        MySqlParameter doctorIdParameter = new MySqlParameter();
        doctorIdParameter.ParameterName = "@DoctorId";
        doctorIdParameter.Value = doctorId;
        doctorQuery.Parameters.Add(doctorIdParameter);
        var doctorQueryRdr = doctorQuery.ExecuteReader() as MySqlDataReader;
        while(doctorQueryRdr.Read())
        {
          int thisDoctorId = doctorQueryRdr.GetInt32(0);
          string doctorDescription = doctorQueryRdr.GetString(1);
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

    public override bool Equals(System.Object otherSpecialty)
    {
      if (!(otherSpecialty is Specialty))
      {
        return false;
      }
      else
      {
        Specialty newSpecialty = (Specialty) otherSpecialty;
        bool idEquality = this.GetId().Equals(newSpecialty.GetId());
        bool nameEquality = this.GetName().Equals(newSpecialty.GetName());
        return (idEquality && nameEquality);
      }
    }

    public void Save()
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = new MySqlCommand(@"INSERT INTO specialty (specialty_name) VALUES (@specialty_name);", conn);
      cmd.Parameters.AddWithValue("@specialty_name", this._specialty);

      cmd.ExecuteNonQuery();
      _id = (int) cmd.LastInsertedId; // <-- This line is new!
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }

    public void DeleteSpecialty(int specialtyId)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      var cmd = conn.CreateCommand() as MySqlCommand;

      Specialty selectedSpecialty = Specialty.Find(specialtyId);
      Dictionary<string, object> model = new Dictionary<string, object>();
      List<Patient> doctorSpecialty = selectedSpecialty.GetDoctors();
      model.Add("specialty", selectedSpecialty);

      foreach (Doctor doctor in doctorSpecialty)
      {
        patient.Delete();
      }

      cmd.CommandText = @"DELETE FROM specialties WHERE id = @thisId;";
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
      MySqlCommand cmd = new MySqlCommand("DELETE FROM specialties WHERE id = @SpecialtyId; DELETE FROM doctors_specialties WHERE specialty_id = @SpecialtyId;", conn);
      cmd.Parameters.AddWithValue("@SpecialtyId", this.GetId());
      cmd.ExecuteNonQuery();
      if (conn != null)
      {
        conn.Close();
      }
    }

    public void AddDoctor (Doctor newDoctor)
    {
      MySqlConnection conn = DB.Connection();
      conn.Open();
      MySqlCommand cmd = new MySqlCommand(@"INSERT INTO doctors_specialties (specialty_id, doctor_id) VALUES (@SpecialtyId, @DoctorId);", conn);
      cmd.Parameters.AddWithValue("@SpecialtyId", _id);
      cmd.Parameters.AddWithValue("@DoctorId", newDoctor.GetId());
      cmd.ExecuteNonQuery();
      conn.Close();
      if (conn != null)
      {
        conn.Dispose();
      }
    }
  }
}
