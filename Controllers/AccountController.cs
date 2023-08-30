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
        if (ModelState.IsValid)
        {
            if (userHelper.IsValidUser(model.Email, model.Password))
            {
                string userRole = userHelper.GetUserRole(model.Email);

                if (userRole == "Admin")
                {
                    FormsAuthentication.SetAuthCookie(model.Email, false);
                    return RedirectToAction("Index", "Admin");
                }
                else if (userRole == "User")
                {
                    FormsAuthentication.SetAuthCookie(model.Email, false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid user role");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid credentials");
            }
        }

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
