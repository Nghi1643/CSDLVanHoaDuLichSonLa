using Domain;

namespace CSDLVanHoaDuLichSonLa.Models
{
    public class AdminNavModel : ICloneable
    {
        public List<AdminNavItemModel> Items { get; set; } = new List<AdminNavItemModel>();

        public object Clone()
        {
            var clonedObj = new AdminNavModel
            {
                Items = new List<AdminNavItemModel>()
            };

            foreach (var item in Items)
            {
                clonedObj.Items.Add((AdminNavItemModel)item.Clone());
            }

            return clonedObj;
        }
    }

    /*
    public class AdminNavItemModel : ICloneable
    {
        public int Id { get; set; }
        public string AreaName { get; set; }
        public string ControllerName {  get; set; }
        public string ActionName {  get; set; }
        public string Title {  get; set; }
        public bool IsAuthorize { get; set; } = false;
        public string Icon { get; set; }
        public bool IsLeaf { get; set; }
        public List<string> ListRoles { get; set; } = new List<string>();
        public List<AdminNavSubItemModel> ListChilds { get; set; } = new List<AdminNavSubItemModel>();

        public object Clone()
        {
            var cloneObj = new AdminNavItemModel
            {
                Id = Id,
                AreaName = AreaName,
                ControllerName = ControllerName,
                ActionName = ActionName,
                Title = Title,
                Icon = Icon,
                IsLeaf = IsLeaf,
                ListRoles = new List<string>(),
                ListChilds = new List<AdminNavSubItemModel>()
            };

            foreach (var item in ListRoles)
                cloneObj.ListRoles.Add(item);

            foreach (var item in ListChilds)
            {
                cloneObj.ListChilds.Add((AdminNavSubItemModel)item.Clone());
            }

            return cloneObj;
        }
    }

    public class AdminNavSubItemModel : ICloneable
    {
        public int Id { get; set; }
        public string AreaName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Title { get; set; }
        public bool IsAuthorize { get; set; } = false;
        public bool IsLeaf { get; set; }
        public List<string> ListRoles { get; set; } = new List<string>();

        public object Clone()
        {
            var clondObj = new AdminNavSubItemModel
            {
                Id = Id,
                AreaName = AreaName,
                ControllerName = ControllerName,
                ActionName = ActionName,
                Title = Title,
                IsLeaf = IsLeaf,
                ListRoles = new List<string>()
            };

            foreach (var role in ListRoles)
                clondObj.ListRoles.Add(role);

            return clondObj;
        }
    }
    */
}
