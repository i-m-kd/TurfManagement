using System.Web.Mvc;
using System.Web.Security;
using TurfManagement.ConnectionHelper;
using TurfManagement.Models;

public class AccountController : Controller
{
    Helper userHelper = new Helper();

    [AllowAnonymous]
    public ActionResult Login()
    {
        LoginModel model = new LoginModel();
        return View(model);
    }

    [HttpPost]
    public ActionResult Login(LoginModel model)
    {

        if (ModelState.IsValid && userHelper.IsValidUser(model.Email, model.Password))
        {
            FormsAuthentication.SetAuthCookie(model.Email, false);
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError("", "Invalid credentials");
        return View(model);
    }
    public ActionResult Logout()
    {
        FormsAuthentication.SignOut();

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    public ActionResult Register()
    {
        RegisterModel model = new RegisterModel();
        return View(model);
    }
    [HttpPost]
    public ActionResult Register(RegisterModel model)
    {
        if (ModelState.IsValid)
        {
            userHelper.InsertUser(model);

            return RedirectToAction("Login");
        }

        return View(model);
    }

}
