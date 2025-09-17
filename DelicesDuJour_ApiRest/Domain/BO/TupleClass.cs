namespace DelicesDuJour_ApiRest.Domain.BO
{
    public class TupleClass<T, V>
    {
        public T t { get; set; }
        public V v { get; set; }

        public TupleClass() { }

        // Constructeur qui prend un tuple en paramètre pour le confort du développeur
        public TupleClass((T t, V v) key)
        {
            this.t = key.t;
            this.v = key.v;
        }
    }
}