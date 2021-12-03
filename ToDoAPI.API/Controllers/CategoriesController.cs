using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ToDoAPI.API.Models;
using ToDoAPI.DATA.EF;
using System.Web.Http.Cors;

namespace ToDoAPI.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class CategoriesController : ApiController
    {
        ToDoEntities db = new ToDoEntities();

        //api/Categories
        public IHttpActionResult GetCategories()
        {
            List<CategoryViewModels> cats = db.Categories.Select(c => new CategoryViewModels()
            {
                CategoryId = c.CategoryId,
                CategoryName = c.Name,
                CategoryDescription = c.Description
            }).ToList<CategoryViewModels>();

            if (cats.Count == 0)
                return NotFound();

            return Ok(cats);
        }//end GetCategories

        //api/Categories/id
        public IHttpActionResult GetCategories(int id)
        {
            CategoryViewModels cat = db.Categories.Where(c => c.CategoryId == id).Select(c => new CategoryViewModels()
            {
                CategoryId = c.CategoryId,
                CategoryName = c.Name,
                CategoryDescription = c.Description
            }).FirstOrDefault();

            if (cat == null)
            {
                return NotFound();
            }

            return Ok(cat);
        }//end GetCategories/id

        //api/Categories (HttpPost)
        public IHttpActionResult PostCategory(CategoryViewModels cat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            db.Categories.Add(new Category()
            {
                Name = cat.CategoryName,
                Description = cat.CategoryDescription
            });

            db.SaveChanges();
            return Ok();

        }//end PostCategory

        //api/Categories (HttpPut)
        public IHttpActionResult PutCategory(CategoryViewModels cat)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            Category existingCategory = db.Categories.Where(c => c.CategoryId == cat.CategoryId).FirstOrDefault();

            if (existingCategory != null)
            {
                existingCategory.Name = cat.CategoryName;
                existingCategory.Description = cat.CategoryDescription;
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }//end Put Category

        //api/Categories/id (HttpDelete)
        public IHttpActionResult DeleteCategory(int id)
        {
            Category cat = db.Categories.Where(c => c.CategoryId == id).FirstOrDefault();

            if (cat != null)
            {
                db.Categories.Remove(cat);
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }//end DeleteCategory

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
