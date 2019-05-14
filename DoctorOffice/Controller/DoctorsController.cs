using System.Collections.Generic;
using System;
using Microsoft.AspNetCore.Mvc;
using DoctorOffice.Models;


namespace DoctorOffice.Controllers
{
  public class DoctorsController : Controller
  {

    [HttpGet("/doctors")]
    public ActionResult Index()
    {
      List<Doctor> allDoctors = Doctor.GetAll();
      return View(allDoctors);
    }

    [HttpGet("/doctors/new")]
    public ActionResult New()
    {
      return View();
    }

    [HttpPost("/doctors")]
    public ActionResult Create(string doctorName)
    {
      Doctor newDoctor = new Doctor(doctorName);
      newDoctor.Save();
      List<Doctor> allDoctors = Doctor.GetAll();

      return View("Index", allDoctors);

    }
    //modified Controller and Show View to accept a single doctor object
    //instead of a Dictionary (we thought dictionary was redundant)

    [HttpGet("/doctors/{id}")]
    public ActionResult Show(int id)
    {
      Dictionary<string, object> model = new Dictionary<string, object>();
      Doctor selectedDoctor = Doctor.Find(id);
      List<Patient> doctorPatients = selectedDoctor.GetPatients();
      List<Patient> allPatients = Patient.GetAll();
      model.Add("doctor", selectedDoctor);
      model.Add("patients", doctorPatients);
      model.Add("allPatients", allPatients);
      return View(model);

    }
    // This one creates new Patients within a given Doctor, not new Doctors:

    [HttpPost("/doctors/{doctorId}/patients")]
    public ActionResult Create(int doctorId, string patientName)
    {
      Dictionary<string, object> model = new Dictionary<string, object>();
      Doctor foundDoctor = Doctor.Find(doctorId);
      Patient newPatient = new Patient(patientName, doctorId);
      newPatient.Save();
      foundDoctor.GetPatients();
      List<Patient> doctorPatients = foundDoctor.GetPatients();
      model.Add("patients", doctorPatients);
      model.Add("doctor", foundDoctor);
      return View("Show", foundDoctor);
    }

    [HttpPost("/doctors/{doctorId}/delete-doctor")]
    public ActionResult DeleteDoctor(int doctorId)
    {
      Doctor selectedDoctor = Doctor.Find(doctorId);
      selectedDoctor.DeleteDoc(doctorId);
      Dictionary<string, object> model = new Dictionary<string, object>();
      List<Patient> doctorPatients = selectedDoctor.GetPatients();
      model.Add("doctor", selectedDoctor);
      return RedirectToAction("Index", "Doctors");

      //
      // Patient patient = Patient.Find(patientId);
      // patient.Delete();
      // Dictionary<string, object> model = new Dictionary<string, object>();
      // Doctor foundDoctor = Doctor.Find(doctorId);
      // List<Patient> doctorPatients = foundDoctor.GetPatients();
      // model.Add("patient", doctorPatients);
      // model.Add("doctor", foundDoctor);
      // // return View(model);
      // return RedirectToAction("Show", "Doctors");
      // //return RedirectToAction("actionName", "controllerName"); goes to a cshtml page in a different controller.
    }

    [HttpPost("/doctors/{doctorId}/patients/new")]
    public ActionResult AddPatient(int doctorId, int patientId)
    {
      Doctor doctor = Doctor.Find(doctorId);
      Patient patient = Patient.Find(patientId);
      doctor.AddPatient(patient);
      return RedirectToAction("Show",  new { id = doctorId });
    }
  }
}
