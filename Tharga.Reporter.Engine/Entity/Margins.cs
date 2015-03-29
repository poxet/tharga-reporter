namespace Tharga.Reporter.Engine.Entity
{
    public class Margins : UnitRectangle
    {
        private Margins(UnitValue left, UnitValue top, UnitValue right, UnitValue bottom)
            : base(left, top, right, bottom)
        {
        }

        public static Margins Create(UnitValue left, UnitValue top, UnitValue right, UnitValue bottom)
        {
            return new Margins(left, top, right, bottom);
        }
    }
}