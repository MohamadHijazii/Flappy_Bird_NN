using System;
using System.Collections.Generic;
using System.Text;
public class Matrix
{
    private double[,] m;
    int l, c;
    int nextL, nextC;

    public Matrix(int l,int c)
    {
    if(l<1 || c < 1)
    {
    throw new Exception();
    }
    this.l = l;
    this.c = c;
    m = new double[l, c];
    nextL = 0;
    nextC = 0;
    }

    public int getColumnsCount() => c;
    public int getLinesCount() => l;

    public double getAt(int i,int j)
    {
    if (i >= l || i < 0 || j >= c || j < 0)
    {
    throw new MatrixException("Error in dimensions");
    }
    return m[i, j];
    }

    public void setAt(int i,int j,double e)
    {
    if(i>=l || i<0 || j >= c || j < 0)
    {
    throw new MatrixException("Error in dimensions");
    }
    m[i, j] = e;
    }

    public double[] getLineAt(int line)
    {
    if(line >= l || line < 0)
    {
    throw new MatrixException("Error in dimensions");
    }
    double[] r = new double[c];
    for(int i = 0; i < c; i++)
    {
    r[i] = m[line, i];
    }
    return r;
    }

    public double[] getColumnAt(int col)
    {
    if (col >= c || col<0)
    {
    throw new MatrixException("Error in dimensions");
    }
    double[] r = new double[l];
    for (int i = 0; i < l; i++)
    {
    r[i] = m[i, col];
    }
    return r;
    }

    public void addLine(double[] ligne)
    {
    if(ligne.Length != c || nextL >= l)
    {
    throw new MatrixException("Error in dimensions");
    }
    for(int i = 0; i < c; i++)
    {
    m[nextL, i] = ligne[i];
    }
    nextL++;
    }

    public void addColumn(double[] col)
    {
    if (col.Length != l || nextC >= c)
    {
    throw new MatrixException("Error in dimensions");
    }
    for (int i = 0; i < l; i++)
    {
    m[i, nextC] = col[i];
    }
    nextC++;
    }

    public Matrix add(Matrix other)
    {
    if (l != other.l || c != other.c)
    {
    throw new MatrixException("Error in dimensions");
    }
    Matrix s = new Matrix(l, c);
    for (int i = 0; i < l; i++)
    {
    for (int j = 0; j < c; j++)
    {
    s.m[i, j] = m[i, j] + other.m[i, j];
    }
    }
    return s;
    }

    public Matrix substract(Matrix other)
    {
    if (l != other.l || c != other.c)
    {
    throw new MatrixException("Error in dimensions");
    }
    Matrix s = new Matrix(l, c);
    for (int i = 0; i < l; i++)
    {
    for (int j = 0; j < c; j++)
    {
    s.m[i, j] = m[i, j] - other.m[i, j];
    }
    }
    return s;
    }

    public Matrix mult(Matrix other)
    {
    if(c != other.l)
    {
    throw new MatrixException("Column in the first matrix should be equal to the " +
    "line of the second");
    }
    Matrix r = new Matrix(l, other.c);

    for(int i = 0; i < l; i++)
    {
    for(int j = 0; j < other.c; j++)
    {
    double[] li = getLineAt(i);
    double[] cj = other.getColumnAt(j);
    double s = 0;
    for(int k = 0; k < li.Length; k++)
    {
    s += li[k] * cj[k];
    }
    r.setAt(i, j, s);
    }
    }


    return r;

    }

    public Matrix add(double n)
    {
        Matrix r = new Matrix(l, c);
        for(int i = 0; i < l; i++)
        {
            for(int j = 0; j < c; j++)
            {
                r.setAt(i, j, n + getAt(i, j));
            }
        }
        return r;
    }

    public void print()
    {
    for(int i = 0; i < l; i++)
    {
    for(int j = 0; j < c; j++)
    {
    Console.Write($"{m[i,j]}\t");
    }
    Console.Write("\n");
    }
    }
}

