/*
  Cassowary.net: an incremental constraint solver for .NET
  (http://lumumba.uhasselt.be/jo/projects/cassowary.net/)
  
  Copyright (C) 2005  Jo Vermeulen (jo.vermeulen@uhasselt.be)
  
  This program is free software; you can redistribute it and/or
  modify it under the terms of the GNU Lesser General Public License
  as published by the Free Software Foundation; either version 2.1
  of  the License, or (at your option) any later version.

  This program is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
  GNU Lesser General Public License for more details.

  You should have received a copy of the GNU Lesser General Public License
  along with this program; if not, write to the Free Software
  Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
*/

using System;

namespace Cassowary
{
  public class ClSymbolicWeight : ICloneable
  {
    public ClSymbolicWeight(int cLevels)
    {
      _values = new double[cLevels];
    }

    public ClSymbolicWeight(double w1, double w2, double w3)
    {
      _values = new double[3];
      _values[0] = w1;
      _values[1] = w2;
      _values[2] = w3;
    }

    public ClSymbolicWeight(double[] weights)
    {
      int cLevels = weights.Length;
      _values = new double[cLevels];
      
      for (int i = 0; i < cLevels; i++)
      {
        _values[i] = weights[i];
      }
    }

    public virtual object Clone()
    {
      return new ClSymbolicWeight(_values);
    }

    public static ClSymbolicWeight operator*(ClSymbolicWeight clsw, double n)
    {
      return clsw.Times(n);
    }

    public static ClSymbolicWeight operator*(double n, ClSymbolicWeight clsw)
    {
      return clsw.Times(n);
    }

    public ClSymbolicWeight Times(double n)
    {
      ClSymbolicWeight clsw = (ClSymbolicWeight) Clone();
      
      for (int i = 0; i < _values.Length; i++) 
      {
        clsw._values[i] *= n;
      }
      
      return clsw;
    }

    public static ClSymbolicWeight operator/(ClSymbolicWeight clsw, double n)
    {
      return clsw.DivideBy(n);
    }

    public static ClSymbolicWeight operator/(double n, ClSymbolicWeight clsw)
    {
      return clsw.DivideBy(n);
    }

    public ClSymbolicWeight DivideBy(double n)
    {
      // Assert(n != 0);
      ClSymbolicWeight clsw = (ClSymbolicWeight) Clone();
      
      for (int i = 0; i < _values.Length; i++)
      {
        clsw._values[i] /= n;
      }

      return clsw;
    }

    public static ClSymbolicWeight operator+(ClSymbolicWeight clsw1, ClSymbolicWeight clsw2)
    {
      return clsw1.Add(clsw2);
    }

    public ClSymbolicWeight Add(ClSymbolicWeight clsw1)
    {
      // Assert(clws.CLevels == CLevels);
      ClSymbolicWeight clsw = (ClSymbolicWeight) Clone();
      
      for (int i = 0; i < _values.Length; i++)
      {
        clsw._values[i] += clsw1._values[i];
      }
      
      return clsw;
    }

    public static ClSymbolicWeight operator-(ClSymbolicWeight clsw1, ClSymbolicWeight clsw2)
    {
      return clsw1.Subtract(clsw2);
    }

    public ClSymbolicWeight Subtract(ClSymbolicWeight clsw1)
    {
      // Assert(clsw1.CLevels == CLevels);
      ClSymbolicWeight clsw = (ClSymbolicWeight) Clone();
      
      for (int i = 0; i < _values.Length; i++)
      {
        clsw._values[i] -= clsw1._values[i];
      }
      
      return clsw;
    }

    // TODO: comparison operators (<, <=, >, >=, ==)
    public bool LessThan(ClSymbolicWeight clsw1)
    {
      // Assert(clsw1.CLevels == CLevels);

      for (int i = 0; i < _values.Length; i++)
      {
        if (_values[i] < clsw1._values[i])
          return true;
        else if (_values[i] > clsw1._values[i])
          return false;
      }

      return false; // they are equal
    }

    public bool LessThanOrEqual(ClSymbolicWeight clsw1)
    {
      // Assert(clsw1.CLevels == CLevels);

      for (int i = 0; i < _values.Length; i++)
      {
        if (_values[i] < clsw1._values[i])
          return true;
        else if (_values[i] > clsw1._values[i])
          return false;
      }

      return true; // they are equal
    }

    public bool Equal(ClSymbolicWeight clsw1)
    {
      for (int i = 0; i < _values.Length; i++)
      {
        if (_values[i] != clsw1._values[i])
          return false;
      }
      
      return true; // they are equal
    }

    public bool GreaterThan(ClSymbolicWeight clsw1)
    {
      return !LessThan(clsw1);
    }

    public bool IsNegative()
    {
      return LessThan(ClsZero);
    }

    public double AsDouble()
    {
      double sum = 0;
      double factor = 1;
      double multiplier = 1000;

      for (int i = _values.Length - 1; i >= 0; i--)
      {
        sum += _values[i] * factor;
        factor *= multiplier;
      }

      return sum;
    }

    public override string ToString()
    {
      string result = "[";

      for (int i = 0; i < _values.Length - 1; i++)
      {
        result += _values[i] + ",";
      }
      
      result += _values[_values.Length - 1] + "]";
      
      return result;
    }
    
    public int CLevels
    {
      get { return _values.Length; }
    }
    
    public static ClSymbolicWeight ClsZero
    {
      get { return _clsZero; }
    }

    private double[] _values;
    
    private static readonly ClSymbolicWeight _clsZero = new ClSymbolicWeight(0.0, 0.0, 0.0);
  }
}
