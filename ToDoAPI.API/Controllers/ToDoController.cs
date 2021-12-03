using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ToDoAPI.DATA.EF;
using ToDoAPI.API.Models;
using System.Web.Http.Cors;

namespace ToDoAPI.API.Controllers
{
    [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class ToDoController : ApiController
    {
        ToDoEntities db = new ToDoEntities();

        //READ functionality
        //api/Resources
        public IHttpActionResult GetToDo()
        {
            //Create list to house resources
            //If this doesn't display, consider installing entity framework on the API layer.
            List<TodoViewModels> todo = db.TodoItems.Include("Category").Select(t => new TodoViewModels()
            {
                //This section of code is translating the database Resource objects to the Data Transfer Objects. This is called abstraction, as we are adding a layer which consuming apps will access instead of accessing the domain models directly.
                ToDoId = t.TodoId,
                Action = t.Action,
                Done = t.Done,
                CategoryId = t.CategoryId == null ? 0 : t.CategoryId,
                Category = new CategoryViewModels()
                {
                    CategoryId = t.Category.CategoryId,
                    CategoryName = t.Category.Name,
                    CategoryDescription = t.Category.Description
                }
            }).ToList<TodoViewModels>();

            //Check on the results and if there are no results we will send back to the consuming app a 404
            if (todo.Count == 0)
            {
                return NotFound();//404 error
            }

            return Ok(todo);
        }//end GetResources

        //api/Resources/id
        //Details
        public IHttpActionResult GetToDo(int id)
        {
            //Create a new ResourceViewModel object and assign it the value of the appropriate resource from the db
            TodoViewModels todo = db.TodoItems.Include("Category").Where(t => t.TodoId == id).Select(t => new TodoViewModels()
            {
                //copy our assignments from above - GetResources
                ToDoId = t.TodoId,
                Action = t.Action,
                Done = t.Done,
                CategoryId = t.CategoryId == null ? 0 : t.CategoryId,
                Category = new CategoryViewModels()
                {
                    CategoryId = t.Category.CategoryId,
                    CategoryName = t.Category.Name,
                    CategoryDescription = t.Category.Description
                }
            }).FirstOrDefault();

            //Check that there is a resource and return the resource with an OK (200) response
            if (todo == null)
            {
                return NotFound();
            }

            return Ok(todo);
        }

        //api/Resources (HttpPost)
        public IHttpActionResult PostToDo(TodoViewModels todo)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid Data");
            }

            TodoItem newToDo = new TodoItem()
            {
                Action = todo.Action,
                Done = todo.Done,
                CategoryId = todo.CategoryId == null ? 0 : todo.CategoryId
            };

            db.TodoItems.Add(newToDo);
            db.SaveChanges();
            return Ok(newToDo);
        }//end PostResource

        //api/Resources (HttpPut)
        public IHttpActionResult PutResource(TodoViewModels todo)
        {
            if (!ModelState.IsValid)
                return BadRequest("Invalid Data");//scopeless if...this is one of the few good examples of a scopeless, with just one line of code in the scope of this if

            //get the resource from the db that we want to edit
            TodoItem existingToDoItems = db.TodoItems.Where(t => t.TodoId == todo.ToDoId).FirstOrDefault();

            //If the resource isn't null, then we will reassign its values from the consuming application's request
            if (existingToDoItems != null)
            {
                existingToDoItems.TodoId = todo.ToDoId;
                existingToDoItems.Action = todo.Action;
                existingToDoItems.Done = todo.Done;
                existingToDoItems.CategoryId = todo.CategoryId;
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }

        }//end PutResouce

        //api/Resouces/id (HttpDelete)
        public IHttpActionResult DeleteToDo(int id)
        {
            TodoItem ToDo = db.TodoItems.Where(t => t.TodoId == id).FirstOrDefault();

            if (ToDo != null)
            {
                db.TodoItems.Remove(ToDo);
                db.SaveChanges();
                return Ok();
            }
            else
            {
                return NotFound();
            }
        }//end DeleteResource

        //We use the Dispose() below to dispose of any connections to the db after we are done with them. Best practice to handle performance - dispose of the instance of the controller and db when done with it.
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();//disposes the db connection
            }
            base.Dispose(disposing);//disposes the instance of the controller
        }
    }
}
