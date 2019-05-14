using Microsoft.AspNetCore.Mvc;
using DoctorOffice.Models;
using System.Collections.Generic;

namespace DoctorOffice.Controllers
{
  public class SpecialtiesController : Controller
  {

    [HttpGet("/specialties")]
    public ActionResult Index()
    {
      List<Specialty> allSpecialties = Specialty.GetAll();
      return View(allSpecialties);
    }

    [HttpGet("/specialties/new")]
    public ActionResult New()
    {
      return View();
    }

    [HttpGet("/specialties/{id}")]
    public ActionResult Show(int id)
    {
      Dictionary<string, object> model = new Dictionary<string, object>();
      Specialty selectedSpecialty = Specialty.Find(id);
      List<Doctor> specialtyDoctors = selectedSpecialty.GetDoctors();
      List<Doctor> allDoctors = Doctor.GetAll();
      model.Add("selectedSpecialty", selectedSpecialty);
      model.Add("specialtyDoctors", specialtyDoctors);
      model.Add("allDoctors", allDoctors);
      return View(model);
    }

    [HttpPost("/specialties")]
    public ActionResult Create(string specialtyName)
    {
      Specialty newSpecialty = new Specialty(specialtyName);
      newSpecialty.Save();
      List<Specialty> allSpecialties = Specialty.GetAll();
      return View("Index", allSpecialties);
    }

    // [HttpGet("/doctors/{doctorId}/specialty/{specialtyId}/edit")]
    // public ActionResult Edit(int doctorId, int specialtyId)
    // {
    //   Dictionary<string, object> model = new Dictionary<string, object>();
    //   Doctor doctor = Doctor.Find(doctorId);
    //   model.Add("doctor", doctor);
    //   Specialty specialty = Specialty.Find(specialtyId);
    //   model.Add("specialty", specialty);
    //   return View(model);
    // }

    [HttpPost("/specialties/{specialtyId}/doctors/new")]
    public ActionResult AddDoctor(int specialtyId, int doctorId)
    {
      Specialty specialty = Specialty.Find(specialtyId);
      Doctor doctor = Doctor.Find(doctorId);
      specialty.AddDoctor(doctor);
      return RedirectToAction("Show",  new { id = specialtyId });
    }

  }
}
