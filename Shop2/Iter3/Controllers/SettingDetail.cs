using Iter3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace Iter3.Controllers
{
    public class SettingDetail : Controller
    {
        private readonly G3_PerfumeShopDB_Iter3Context _context;

        public SettingDetail(G3_PerfumeShopDB_Iter3Context context)
        {
            _context = context;
        }

        public IActionResult Index(int userId)
        {
            var user = _context.Users
                .Include(u => u.Role)
                .Include(u => u.UserRolesAuditUsers)
                    .ThenInclude(audit => audit.OldRole)
                .Include(u => u.UserRolesAuditUsers)
                    .ThenInclude(audit => audit.NewRole)
                .Include(u => u.UserRolesAuditUsers)
                    .ThenInclude(audit => audit.ChangedByNavigation)
                .FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return NotFound();
            }

            var availableRoles = _context.Roles
                .Where(r => r.Id != 1 && r.Id != 4)
                .ToList();

            // Lấy bản ghi mới nhất của UserRolesAudit
            var latestAuditRecord = user.UserRolesAuditUsers
                .OrderByDescending(audit => audit.Id)
                .FirstOrDefault();

            ViewBag.AvailableRoles = availableRoles;
            ViewBag.UserId = user.Id;
            ViewBag.LatestAuditRecord = latestAuditRecord;

            return View(user.UserRolesAuditUsers);
        }

        [HttpPost]
        public IActionResult UpdateRole(int userId, int newRoleId, string note)
        {
            var user = _context.Users.FirstOrDefault(u => u.Id == userId);
            if (user == null) return NotFound();

            if (newRoleId == 1 || newRoleId == 4)
            {
                ModelState.AddModelError("", "Không thể thay đổi thành vai trò Admin hoặc Customer.");
                return RedirectToAction("Index", new { userId });
            }

            // Lấy bản ghi mới nhất của UserRolesAudit cho người dùng
            var auditRecord = _context.UserRolesAudits
                .Where(a => a.UserId == userId)
                .OrderByDescending(a => a.Id)
                .FirstOrDefault();

            if (auditRecord != null)
            {
                // Cập nhật bản ghi hiện có
                auditRecord.NewRoleId = newRoleId;
                auditRecord.ChangedBy = 1; // ID người thực hiện thay đổi
                auditRecord.ChangeDate = DateTime.Now;
                auditRecord.Note = note;
            }
            else
            {
                // Nếu không có bản ghi nào, tạo một bản ghi mới
                auditRecord = new UserRolesAudit
                {
                    UserId = userId,
                    OldRoleId = user.RoleId,
                    NewRoleId = newRoleId,
                    ChangedBy = 1,
                    ChangeDate = DateTime.Now,
                    Note = note
                };
                _context.UserRolesAudits.Add(auditRecord);
            }

            // Cập nhật RoleId cho user
            user.RoleId = newRoleId;
            _context.SaveChanges();

            // Quay lại Index để tải lại dữ liệu mới
            return RedirectToAction("Index", new { userId });
        }
    }
}
