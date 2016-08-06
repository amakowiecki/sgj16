using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SGJ16.Common
{
//    public class Angle
//    {
//		public Angle();
//        public Angle(float angle);
//        public Angle(const Angle& angle);

//		public operator float() const;
//        public Angle operator+(const Angle& angle);
//		public Angle operator-(const Angle& angle);
//		public Angle operator=(const Angle& angle);
//		public Angle operator+=(const Angle& angle);
//		public Angle operator-=(const Angle& angle);
//		public Angle operator+(float angle);
//        public Angle operator-(float angle);
//        public Angle operator=(float angle);
//        public Angle operator+=(float angle);
//        public Angle operator-=(float angle);
//        public bool operator ==(const Angle& angle) const;
//        public bool Angle::equalsZero() const;
//        public bool Angle::equalsPi() const;

//        public bool isBetween(const Angle& angle1, const Angle& angle2) const;

//        private:
//		float radians;
//        float normalize(float angle) const;

//        static const float _ANGLE_PRECISION;
//    };

//    const float Angle::_ANGLE_PRECISION = 0.0001f;
//float Angle::normalize(float angle) const { return std::fmodf(angle + 3 * FLOAT_PI, 2 * FLOAT_PI) - FLOAT_PI; }
//Angle::operator float() const { return radians; }
//Angle::Angle() : radians(0) { }
//Angle::Angle(float angle) : radians(normalize(angle)) {}
//Angle::Angle(const Angle& angle) : radians(angle.radians) { }
//Angle Angle::operator+(const Angle& angle) { return Angle(radians + angle.radians); }
//Angle Angle::operator-(const Angle& angle) { return Angle(radians - angle.radians); }
//Angle& Angle::operator=(const Angle& angle) { radians = angle.radians; return *this; }
//Angle& Angle::operator+=(const Angle& angle) { radians = normalize(radians + angle.radians); return *this; }
//Angle& Angle::operator-=(const Angle& angle) { radians = normalize(radians - angle.radians); return *this; }
//Angle Angle::operator+(float angle) { return Angle(radians + angle); }
//Angle Angle::operator-(float angle) { return Angle(radians - angle); }
//Angle& Angle::operator=(float angle) { radians = normalize(angle); return *this; }
//Angle& Angle::operator+=(float angle) { radians = normalize(radians + angle); return *this; }
//Angle& Angle::operator-=(float angle) { radians = normalize(radians - angle); return *this; }
//bool Angle::operator==(const Angle& angle) const { return fabsf(normalize(radians - angle.radians)) < _ANGLE_PRECISION; }
//bool Angle::equalsZero() const { return fabsf(radians) < _ANGLE_PRECISION; }
//bool Angle::equalsPi() const { return fabsf(radians + FLOAT_PI) < _ANGLE_PRECISION || fabsf(radians - FLOAT_PI) < _ANGLE_PRECISION; }

//Angle radians(float radians) { return Angle(radians); }
//Angle degrees(float degrees) { return Angle(FLOAT_PI * degrees / 180); }
//float measure(const Angle& from, const Angle& to)
//{
//    if (to > from) return to - from;
//    else return 2 * FLOAT_PI + to - from;
//}
//bool Angle::isBetween(const Angle& angle1, const Angle& angle2) const {
//	return measure(angle1, *this) <= measure(angle1, angle2);
//}
}
