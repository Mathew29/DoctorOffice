using Microsoft.AspNetCore.Mvc;
using DoctorOffice.Models;
using System.Collections.Generic;

namespace DoctorOffice.Controllers
{
  public class PatientsController : Controller
  {

    [HttpGet("/patients")]
    public ActionResult Index()
    {
      List<Patient> allPatients = Patient.GetAll();
      return View(allPatients);
    }

    [HttpGet("/patients/new")]
    public ActionResult New()
    {
      return View();
    }

    [HttpPost("/patients")]
    public ActionResult Create(string name)
    {
      Patient newPatient = new Patient(name);
      newPatient.Save();
      List<Patient> allPatients = Patient.GetAll();
      return View("Index", allPatients);
    }

    [HttpGet("/patients/{id}")]
    public ActionResult Show(int id)
    {
      Dictionary<string, object> model = new Dictionary<string, object>();
      Patient selectedPatient = Patient.Find(id);
      List<Doctor> patientDoctors = selectedPatient.GetDoctors();
      List<Doctor> allDoctors = Doctor.GetAll();
      model.Add("selectedPatient", selectedPatient);
      model.Add("patientDoctors", patientDoctors);
      model.Add("allDoctors", allDoctors);
      return View(model);
    }

    [HttpPost("/patients/delete")]
    public ActionResult DeleteAll()
    {
      Patient.ClearAll();
      return View();
    }

    [HttpGet("/doctors/{doctorId}/patients/{patientId}/edit")]
    public ActionResult Edit(int doctorId, int patientId)
    {
      Dictionary<string, object> model = new Dictionary<string, object>();
      Doctor doctor = Doctor.Find(doctorId);
      model.Add("doctor", doctor);
      Patient patient = Patient.Find(patientId);
      model.Add("patient", patient);
      return View(model);
    }

    [HttpPost("/doctors/{doctorId}/patients/{patientId}")]
    public ActionResult Update(int doctorId, int patientId, string newName)
    {
      Patient patient = Patient.Find(patientId);
      patient.Edit(newName);
      Dictionary<string, object> model = new Dictionary<string, object>();
      Doctor doctor = Doctor.Find(doctorId);
      model.Add("doctor", doctor);
      model.Add("patient", patient);
      return View("Show", model);
    }

    [HttpPost("/doctors/{doctorId}/patients/{patientId}/delete-patient")]
    public ActionResult DeletePatient(int doctorId, int patientId)
    {
      Patient patient = Patient.Find(patientId);
      patient.Delete();
      Dictionary<string, object> model = new Dictionary<string, object>();
      Doctor foundDoctor = Doctor.Find(doctorId);
      List<Patient> doctorPatients = foundDoctor.GetPatients();
      model.Add("patient", doctorPatients);
      model.Add("doctor", foundDoctor);
      return RedirectToAction("Show", "Categories");
      //return RedirectToAction("actionName", "controllerName"); goes to a cshtml page in a different controller.
    }

    [HttpPost("/patients/{patientId}/doctors/new")]
    public ActionResult AddDoctor(int patientId, int doctorId)
    {
      Patient patient = Patient.Find(patientId);
      Doctor doctor = Doctor.Find(doctorId);
      patient.AddDoctor(doctor);
      return RedirectToAction("Show",  new { id = patientId });
    }
  }
}
