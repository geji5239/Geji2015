using System; 

namespace Panasia.Core
{ 
    public partial class TableEntity : Entity 
    {
        public TableEntity()
        {
        }

        public TableEntity(string tableName)
            : this()
        {
            Table = tableName;
        }

        public TableEntity(string tableName, string moduleName) : this()
        {
            Table = tableName;
            Module = moduleName;
        }
         
        public string Table { get; private set; }
         
        public string Module { get; private set; }
         
        public EntitySet Collection
        {
            get { return EntitySet.Create(Module, Table); }
        }
    }
}