using System.ComponentModel.DataAnnotations;

namespace FoodDispatchSystem.Web.Models;

public enum OrderType
{
    [Display(Name = "Mesa")]
    DineIn = 1,

    [Display(Name = "Para llevar")]
    Takeaway = 2,

    [Display(Name = "Delivery")]
    Delivery = 3
}