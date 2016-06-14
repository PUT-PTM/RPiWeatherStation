using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Weatherbackend.Models;

namespace Weatherbackend.Controllers
{
    public class MeasurementsController : ApiController
    {
        private WeatherbackendContext db = new WeatherbackendContext();

        // GET: api/Measurements
        public IQueryable<Measurements> GetMeasurements()
        {
            return db.Measurements;
        }

        // GET: api/Measurements/5
        [ResponseType(typeof(Measurements))]
        public async Task<IHttpActionResult> GetMeasurements(int id)
        {
            Measurements measurements = await db.Measurements.FindAsync(id);
            if (measurements == null)
            {
                return NotFound();
            }

            return Ok(measurements);
        }

        // PUT: api/Measurements/5
        [ResponseType(typeof(void))]
        public async Task<IHttpActionResult> PutMeasurements(int id, Measurements measurements)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != measurements.Id)
            {
                return BadRequest();
            }

            db.Entry(measurements).State = EntityState.Modified;

            try
            {
                await db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MeasurementsExists(id))
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

        // POST: api/Measurements
        [ResponseType(typeof(Measurements))]
        public async Task<IHttpActionResult> PostMeasurements(Measurements measurements)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.Measurements.Add(measurements);
            await db.SaveChangesAsync();

            return CreatedAtRoute("DefaultApi", new { id = measurements.Id }, measurements);
        }

        // DELETE: api/Measurements/5
        [ResponseType(typeof(Measurements))]
        public async Task<IHttpActionResult> DeleteMeasurements(int id)
        {
            Measurements measurements = await db.Measurements.FindAsync(id);
            if (measurements == null)
            {
                return NotFound();
            }

            db.Measurements.Remove(measurements);
            await db.SaveChangesAsync();

            return Ok(measurements);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool MeasurementsExists(int id)
        {
            return db.Measurements.Count(e => e.Id == id) > 0;
        }
    }
}