namespace compiler.Syntactic
{
    public class NodeTree
    {
        private string dato;
        private NodeTree hijo;
        private NodeTree hermano;
        private int AuxIt = 0;
        public string Dato { get => dato; set => dato = value; }
        public NodeTree Hijo { get => hijo; set => hijo = value; }
        public NodeTree Hermano { get => hermano; set => hermano = value; }
        public int AuxIteracion { get => AuxIt; set => AuxIt = value; }
        public NodeTree()
        {
            dato = "";
            hijo = null;
            hermano = null;
            AuxIteracion = 0;
        }
    }
}
