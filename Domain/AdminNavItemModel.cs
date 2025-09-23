
namespace Domain
{
    public class AdminNavItemModel : ICloneable
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
        public string AreaName { get; set; }
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string Title { get; set; }
        public bool IsAuthorize { get; set; } = false;
        public string Icon { get; set; }
        public bool IsLeaf { get; set; }
        public bool IsShow { get; set; }
        public List<string> ListRoles { get; set; } = new List<string>();
        public List<AdminNavItemModel> ListChilds { get; set; } = new List<AdminNavItemModel>();

        public object Clone()
        {
            var cloneObj = new AdminNavItemModel
            {
                Id = Id,
                ParentId = ParentId,
                AreaName = AreaName,
                ControllerName = ControllerName,
                ActionName = ActionName,
                Title = Title,
                Icon = Icon,
                IsLeaf = IsLeaf,
                IsShow = IsShow,
                ListRoles = new List<string>(),
                ListChilds = new List<AdminNavItemModel>()
            };

            foreach (var item in ListRoles)
                cloneObj.ListRoles.Add(item);

            foreach (var item in ListChilds)
            {
                cloneObj.ListChilds.Add((AdminNavItemModel)item.Clone());
            }

            return cloneObj;
        }
    }
}
