﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using PhongKham.Core.Entities;

namespace PhongKham.Web.Areas.Identity.Pages.Role
{
    public class UserModel : PageModel
    {
        const int USER_PER_PAGE = 10;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<UserAccount> _userManager;

        public UserModel(RoleManager<IdentityRole> roleManager,
                          UserManager<UserAccount> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public class UserInList : UserAccount
        {
            // Liệt kê các Role của User ví dụ: "Admin,Editor" ...
            public string listroles { set; get; }
        }

        public List<UserInList> users;
        public int totalPages { set; get; }

        [TempData] // Sử dụng Session
        public string StatusMessage { get; set; }

        [BindProperty(SupportsGet = true)]
        public int pageNumber { set; get; }

        public IActionResult OnPost() => NotFound("Cấm post");

        public async Task<IActionResult> OnGet()
        {


            if (pageNumber == 0)
                pageNumber = 1;

            var lusers = (from u in _userManager.Users
                          orderby u.UserName
                          select new UserInList()
                          {
                              Id = u.Id,
                              UserName = u.UserName,
                          });


            int totalUsers = await lusers.CountAsync();


            totalPages = (int)Math.Ceiling((double)totalUsers / USER_PER_PAGE);

            users = await lusers.Skip(USER_PER_PAGE * (pageNumber - 1)).Take(USER_PER_PAGE).ToListAsync();

            // users.ForEach(async (user) => {
            //     var roles = await _userManager.GetRolesAsync(user);
            //     user.listroles = string.Join(",", roles.ToList());
            // });

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.listroles = string.Join(",", roles.ToList());
            }

            return Page();
        }
    }
}
