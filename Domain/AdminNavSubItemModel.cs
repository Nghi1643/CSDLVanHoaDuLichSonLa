
namespace Domain
{
    public class AdminNavSubItemModel : ICloneable
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }
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
                ParentId = ParentId,
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
}
