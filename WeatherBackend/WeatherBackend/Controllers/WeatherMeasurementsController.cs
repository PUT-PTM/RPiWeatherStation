using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using WeatherBackend.Models;

namespace WeatherBackend.Controllers
{
    public class WeatherMeasurementsController : ApiController
    {
        private WeatherMeasurementsDBContext db = new WeatherMeasurementsDBContext();

        // GET: api/WeatherMeasurements
        public IQueryable<WeatherMeasurements> GetWeather()
        {
            return db.Weather;
        }

        // GET: api/WeatherMeasurements/5
        [ResponseType(typeof(WeatherMeasurements))]
        public IHttpActionResult GetWeatherMeasurements(int id)
        {
            WeatherMeasurements weatherMeasurements = db.Weather.Find(id);
            if (weatherMeasurements == null)
            {
                return NotFound();
            }

            return Ok(weatherMeasurements);
        }

        // PUT: api/WeatherMeasurements/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutWeatherMeasurements(int id, WeatherMeasurements weatherMeasurements)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != weatherMeasurements.Id)
            {
                return BadRequest();
            }

            db.Entry(weatherMeasurements).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!WeatherMeasurementsExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/WeatherMeasurements
        [ResponseType(typeof(WeatherMeasurements))]
        public IHttpActionResult PostWeatherMeasurements(WeatherMeasurements weatherMeasurements)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Weather.Add(weatherMeasurements);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = weatherMeasurements.Id }, weatherMeasurements);
        }

        // DELETE: api/WeatherMeasurements/5
        [ResponseType(typeof(WeatherMeasurements))]
        public IHttpActionResult DeleteWeatherMeasurements(int id)
        {
            WeatherMeasurements weatherMeasurements = db.Weather.Find(id);
            if (weatherMeasurements == null)
            {
                return NotFound();
            }

            db.Weather.Remove(weatherMeasurements);
            db.SaveChanges();

            return Ok(weatherMeasurements);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool WeatherMeasurementsExists(int id)
        {
            return db.Weather.Count(e => e.Id == id) > 0;
        }
    }
}