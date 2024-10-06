using API.Data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class BuggyController(DataContext dataContext) :BaseApiController{

    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetAuth(){
        return "secrete string";
    }

    [HttpGet("not-found")]
    public ActionResult<AppUser> GetNotFound()
    {
        return NotFound();
    }

    [HttpGet("server-error")]
    public ActionResult<string> GetServerError()
    {
        throw new Exception("Something went wrong in the serveer");
        return null;
    }

    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {
        return BadRequest("This was not a good request");
    }
}