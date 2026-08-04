#:package System.Drawing.Common@8.0.10
#:property TargetFramework=net10.0-windows

using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

int W = 1100, H = 560, SS = 2;
int w = W * SS, h = H * SS;
int n = w * h;

float[] cr = new float[n], cg = new float[n], cb = new float[n], ca = new float[n];

static int Clamp(int v, int lo, int hi) => v < lo ? lo : (v > hi ? hi : v);

(float r, float g, float b) Ramp(double f)
{
    (double r, double g, double b) purple = (0.55, 0.27, 0.82);
    (double r, double g, double b) blue = (0.21, 0.47, 0.88);
    (double r, double g, double b) green = (0.24, 0.76, 0.57);
    double t;
    (double r, double g, double b) a, c;
    if (f < 0.5) { t = f / 0.5; a = purple; c = blue; }
    else { t = (f - 0.5) / 0.5; a = blue; c = green; }
    t = t * t * (3 - 2 * t);
    return ((float)(a.r + (c.r - a.r) * t), (float)(a.g + (c.g - a.g) * t), (float)(a.b + (c.b - a.b) * t));
}

void Splat(double px, double py, double rad, float r, float g, float b, float alpha)
{
    int x0 = Clamp((int)Math.Floor(px - rad), 0, w - 1);
    int x1 = Clamp((int)Math.Ceiling(px + rad), 0, w - 1);
    int y0 = Clamp((int)Math.Floor(py - rad), 0, h - 1);
    int y1 = Clamp((int)Math.Ceiling(py + rad), 0, h - 1);
    for (int y = y0; y <= y1; y++)
    {
        for (int x = x0; x <= x1; x++)
        {
            double dx = x - px, dy = y - py;
            double d = Math.Sqrt(dx * dx + dy * dy) / rad;
            if (d >= 1) { continue; }
            double t = 1 - d;
            double s = t * t * (3 - 2 * t);
            float aa = (float)(alpha * s);
            if (aa <= 0) { continue; }
            int i = y * w + x;
            float dst = ca[i];
            float outA = aa + dst * (1 - aa);
            if (outA <= 0) { continue; }
            cr[i] = (r * aa + cr[i] * dst * (1 - aa)) / outA;
            cg[i] = (g * aa + cg[i] * dst * (1 - aa)) / outA;
            cb[i] = (b * aa + cb[i] * dst * (1 - aa)) / outA;
            ca[i] = outA;
        }
    }
}

double sx = -0.10 * w, sy = -0.30 * h;
double ex = 1.18 * w, ey = 1.35 * h;
double axv = ex - sx, ayv = ey - sy;
double len = Math.Sqrt(axv * axv + ayv * ayv);
double ux = axv / len, uy = ayv / len;
double vx = -uy, vy = ux;
double R = 0.23 * h;
double turns = 4.5;
double tmax = turns * 2 * Math.PI;
int steps = 9000;

for (int pass = 0; pass < 2; pass++)
{
    for (int i = 0; i <= steps; i++)
    {
        double t = tmax * i / steps;
        double z = Math.Sin(t);
        if (pass == 0 && z >= 0) { continue; }
        if (pass == 1 && z < 0) { continue; }
        double s = len * i / steps;
        double px = sx + ux * s + vx * R * Math.Cos(t);
        double py = sy + uy * s + vy * R * Math.Cos(t);
        double rad = 0.055 * h * (1 + 0.35 * z);
        double f = (double)i / steps;
        var col = Ramp(f);
        float alpha = (float)(0.45 + 0.55 * (z + 1) / 2);
        Splat(px, py, rad, col.r, col.g, col.b, alpha);
    }
}

for (int i = 0; i < n; i++)
{
    cr[i] *= ca[i];
    cg[i] *= ca[i];
    cb[i] *= ca[i];
}

void BoxBlur(float[] src, int radius)
{
    float[] tmp = new float[n];
    double denom = 2 * radius + 1;
    for (int y = 0; y < h; y++)
    {
        int row = y * w;
        double sum = 0;
        for (int x = -radius; x <= radius; x++) { sum += src[row + Clamp(x, 0, w - 1)]; }
        for (int x = 0; x < w; x++)
        {
            tmp[row + x] = (float)(sum / denom);
            sum += src[row + Clamp(x + radius + 1, 0, w - 1)] - src[row + Clamp(x - radius, 0, w - 1)];
        }
    }

    for (int x = 0; x < w; x++)
    {
        double sum = 0;
        for (int y = -radius; y <= radius; y++) { sum += tmp[Clamp(y, 0, h - 1) * w + x]; }
        for (int y = 0; y < h; y++)
        {
            src[y * w + x] = (float)(sum / denom);
            sum += tmp[Clamp(y + radius + 1, 0, h - 1) * w + x] - tmp[Clamp(y - radius, 0, h - 1) * w + x];
        }
    }
}

int blurRadius = 40;
for (int pass = 0; pass < 3; pass++)
{
    BoxBlur(cr, blurRadius);
    BoxBlur(cg, blurRadius);
    BoxBlur(cb, blurRadius);
    BoxBlur(ca, blurRadius);
}

double opacity = 0.34;
byte[] pixels = new byte[W * H * 4];
for (int y = 0; y < H; y++)
{
    for (int x = 0; x < W; x++)
    {
        double pr = 0, pg = 0, pb = 0, pa = 0;
        for (int oy = 0; oy < SS; oy++)
        {
            for (int ox = 0; ox < SS; ox++)
            {
                int i = (y * SS + oy) * w + (x * SS + ox);
                pr += cr[i]; pg += cg[i]; pb += cb[i]; pa += ca[i];
            }
        }

        double inv = 1.0 / (SS * SS);
        pr *= inv; pg *= inv; pb *= inv; pa *= inv;

        double d = 0.5 * (x / (double)W) + 0.5 * (y / (double)H);
        double m = (d - 0.26) / (0.92 - 0.26);
        m = m < 0 ? 0 : (m > 1 ? 1 : m);
        m = m * m * (3 - 2 * m);
        double k = m * opacity;

        double outR = 1.0 * (1 - pa * k) + pr * k;
        double outG = 1.0 * (1 - pa * k) + pg * k;
        double outB = 1.0 * (1 - pa * k) + pb * k;

        int o = (y * W + x) * 4;
        pixels[o + 0] = (byte)Math.Round(Math.Min(1, Math.Max(0, outB)) * 255);
        pixels[o + 1] = (byte)Math.Round(Math.Min(1, Math.Max(0, outG)) * 255);
        pixels[o + 2] = (byte)Math.Round(Math.Min(1, Math.Max(0, outR)) * 255);
        pixels[o + 3] = 255;
    }
}

string outPath = args.Length > 0 ? args[0] : @"helix-background.png";
using (var bmp = new Bitmap(W, H, PixelFormat.Format32bppArgb))
{
    var data = bmp.LockBits(new Rectangle(0, 0, W, H), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
    for (int y = 0; y < H; y++)
    {
        Marshal.Copy(pixels, y * W * 4, data.Scan0 + y * data.Stride, W * 4);
    }

    bmp.UnlockBits(data);
    bmp.Save(outPath, ImageFormat.Png);
}

Console.WriteLine("Wrote " + outPath);