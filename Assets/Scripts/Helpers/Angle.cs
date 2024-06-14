/*
Angle.cs

Copyright (c) 2021 yutorisan

This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using System;
using UnityEngine;

namespace UnityUtility
{
    /// <summary>
    /// �p�x
    /// </summary>
    public readonly struct Angle : IEquatable<Angle>, IComparable<Angle>
    {
        /// <summary>
        /// ���K�����Ă��Ȃ��p�x�̗ݐϒl
        /// </summary>
        private readonly float m_totalDegree;

        /// <summary>
        /// �p�x��x���@�Ŏw�肵�āA�V�K�C���X�^���X���쐬���܂��B
        /// </summary>
        /// <param name="angle">�x���@�̊p�x</param>
        /// <exception cref="NotFiniteNumberException"/>
        private Angle(float angle) => m_totalDegree = ArithmeticCheck(() => angle);

        /// <summary>
        /// ���񐔂Ɗp�x���w�肵�āA�V�K�C���X�^���X���쐬���܂��B
        /// </summary>
        /// <param name="lap">����</param>
        /// <param name="angle">�x���@�̊p�x</param>
        /// <exception cref="NotFiniteNumberException"/>
        /// <exception cref="OverflowException"/>
        private Angle(int lap, float angle) => m_totalDegree = ArithmeticCheck(() => checked(360 * lap + angle));

        /// <summary>
        /// �x���@�̒l���g�p���ĐV�K�C���X�^���X���擾���܂��B
        /// </summary>
        /// <param name="degree">�x���@�̊p�x(��)</param>
        /// <returns></returns>
        /// <exception cref="NotFiniteNumberException"/>
        public static Angle FromDegree(float degree) => new Angle(degree);

        /// <summary>
        /// ���񐔂Ɗp�x���w�肵�āA�V�K�C���X�^���X���擾���܂��B
        /// </summary>
        /// <param name="lap">����</param>
        /// <param name="degree">�x���@�̊p�x(��)</param>
        /// <returns></returns>
        /// <exception cref="NotFiniteNumberException"/>
        public static Angle FromDegree(int lap, float degree) => new Angle(lap, degree);

        /// <summary>
        /// �ʓx�@�̒l���g�p���ĐV�K�C���X�^���X���擾���܂��B
        /// </summary>
        /// <param name="radian">�ʓx�@�̊p�x(rad)</param>
        /// <returns></returns>
        /// <exception cref="NotFiniteNumberException"/>
        public static Angle FromRadian(float radian) => new Angle(RadToDeg(radian));

        /// <summary>
        /// ���񐔂Ɗp�x���w�肵�āA�V�K�C���X�^���X���擾���܂��B
        /// </summary>
        /// <param name="lap">����</param>
        /// <param name="radian">�ʓx�@�̊p�x(rad)</param>
        /// <returns></returns>
        /// <exception cref="NotFiniteNumberException"/>
        public static Angle FromRadian(int lap, float radian) => new Angle(lap, RadToDeg(radian));

        /// <summary>
        /// �p�x0���̐V�K�C���X�^���X���擾���܂��B
        /// </summary>
        public static Angle Zero => new Angle(0);

        /// <summary>
        /// �p�x360���̐V�K�C���X�^���X���擾���܂��B
        /// </summary>
        public static Angle Round => new Angle(360);

        public bool Equals(Angle other) => m_totalDegree == other.m_totalDegree;

        public override int GetHashCode() => -1748791360 + m_totalDegree.GetHashCode();

        public override string ToString() => $"{Lap}x + {m_totalDegree - Lap * 360}��";

        public override bool Equals(object obj)
        {
            if (obj is Angle angle) return Equals(angle);
            else return false;
        }

        public int CompareTo(Angle other) => m_totalDegree.CompareTo(other.m_totalDegree);

        /// <summary>
        /// ���K�����ꂽ�p�x(-180�� &lt; degree &lt;= 180��)���擾���܂��B
        /// </summary>
        /// <returns></returns>
        public Angle Normalize() => new Angle(NormalizedDegree);

        /// <summary>
        /// ���̒l�Ő��K�����ꂽ�p�x(0�� &lt;= degree &lt; 360��)���擾���܂��B
        /// </summary>
        /// <returns></returns>
        public Angle PositiveNormalize() => new Angle(PositiveNormalizedDegree);

        /// <summary>
        /// �����𔽓]�������p�x���擾���܂��B
        /// ��F90����-270��, -450����630��
        /// </summary>
        /// <returns></returns>
        public Angle Reverse()
        {
            //�[���Ȃ�[��
            if (this == Zero) return Zero;
            //�^�~�̏ꍇ�͐^�t�ɂ���
            if (IsTrueCircle) return new Angle(-Lap, 0);
            if (IsCircled)
            { //1���ȏサ�Ă���
                if (IsPositive)
                { //360~
                    return new Angle(-Lap, NormalizedDegree - 360);
                }
                else
                { //~-360
                    return new Angle(-Lap, NormalizedDegree + 360);
                }
            }
            else
            { //1�����Ă��Ȃ�
                if (IsPositive)
                { //0~360
                    return new Angle(m_totalDegree - 360);
                }
                else
                { //-360~0
                    return new Angle(m_totalDegree + 360);
                }
            }
        }

        /// <summary>
        /// �����𔽓]�������p�x���擾���܂��B
        /// </summary>
        /// <returns></returns>
        public Angle SignReverse() => new Angle(-m_totalDegree);

        /// <summary>
        /// �p�x�̐�Βl���擾���܂��B
        /// </summary>
        /// <returns></returns>
        public Angle Absolute() => IsPositive ? this : SignReverse();

        /// <summary>
        /// ���K�����Ă��Ȃ��p�x�l���擾���܂��B
        /// </summary>
        public float TotalDegree => m_totalDegree;

        /// <summary>
        /// ���K�����Ă��Ȃ��p�x�l�����W�A���Ŏ擾���܂��B
        /// </summary>
        public float TotalRadian => DegToRad(TotalDegree);

        /// <summary>
        /// ���K�����ꂽ�p�x�l(-180 &lt; angle &lt;= 180)���擾���܂��B
        /// </summary>
        public float NormalizedDegree
        {
            get
            {
                float lapExcludedDegree = m_totalDegree - (Lap * 360);
                if (lapExcludedDegree > 180) return lapExcludedDegree - 360;
                if (lapExcludedDegree <= -180) return lapExcludedDegree + 360;
                return lapExcludedDegree;
            }
        }

        /// <summary>
        /// ���K�����ꂽ�p�x�l�����W�A��(-�� &lt; rad &lt; ��)�Ŏ擾���܂��B
        /// </summary>
        public float NormalizedRadian => DegToRad(NormalizedDegree);

        /// <summary>
        /// ���K�����ꂽ�p�x�l(0 &lt;= angle &lt; 360)���擾���܂��B
        /// </summary>
        public float PositiveNormalizedDegree
        {
            get
            {
                var normalized = NormalizedDegree;
                return normalized >= 0 ? normalized : normalized + 360;
            }
        }

        /// <summary>
        /// ���K�����ꂽ�p�x�l�����W�A��(0 &lt;= rad &lt; 2��)�Ŏ擾���܂��B
        /// </summary>
        public float PositiveNormalizedRadian => DegToRad(PositiveNormalizedDegree);

        /// <summary>
        /// �p�x���������Ă��邩���擾���܂��B
        /// ��F370����1��, -1085����-3��
        /// </summary>
        public int Lap => ((int)m_totalDegree) / 360;

        /// <summary>
        /// 1���ȏサ�Ă��邩�ǂ���(360���ȏ�A��������-360���ȉ����ǂ���)���擾���܂��B
        /// </summary>
        public bool IsCircled => Lap != 0;

        /// <summary>
        /// 360�̔{���̊p�x�ł��邩�ǂ������擾���܂��B
        /// </summary>
        public bool IsTrueCircle => IsCircled && m_totalDegree % 360 == 0;

        /// <summary>
        /// ���̊p�x���ǂ������擾���܂��B
        /// </summary>
        public bool IsPositive => m_totalDegree >= 0;

        /// <exception cref="NotFiniteNumberException"/>
        public static Angle operator +(Angle left, Angle right) => new Angle(ArithmeticCheck(() => left.m_totalDegree + right.m_totalDegree));

        /// <exception cref="NotFiniteNumberException"/>
        public static Angle operator -(Angle left, Angle right) => new Angle(ArithmeticCheck(() => left.m_totalDegree - right.m_totalDegree));

        /// <exception cref="NotFiniteNumberException"/>
        public static Angle operator *(Angle left, float right) => new Angle(ArithmeticCheck(() => left.m_totalDegree * right));

        /// <exception cref="NotFiniteNumberException"/>
        public static Angle operator /(Angle left, float right) => new Angle(ArithmeticCheck(() => left.m_totalDegree / right));

        public static bool operator ==(Angle left, Angle right) => left.m_totalDegree == right.m_totalDegree;

        public static bool operator !=(Angle left, Angle right) => left.m_totalDegree != right.m_totalDegree;

        public static bool operator >(Angle left, Angle right) => left.m_totalDegree > right.m_totalDegree;

        public static bool operator <(Angle left, Angle right) => left.m_totalDegree < right.m_totalDegree;

        public static bool operator >=(Angle left, Angle right) => left.m_totalDegree >= right.m_totalDegree;

        public static bool operator <=(Angle left, Angle right) => left.m_totalDegree <= right.m_totalDegree;

        /// <summary>
        /// ���Z���ʂ����l�ł��邱�Ƃ��m���߂�
        /// </summary>
        /// <param name="func"></param>
        /// <returns></returns>
        private static float ArithmeticCheck(Func<float> func)
        {
            var ans = func();
            if (float.IsInfinity(ans)) throw new NotFiniteNumberException("���Z�̌��ʁA�p�x�����̖�����܂��͕��̖�����ɂȂ�܂���");
            if (float.IsNaN(ans)) throw new NotFiniteNumberException("���Z�̌��ʁA�p�x��NaN�ɂȂ�܂���");
            return ans;
        }

        private static float RadToDeg(float rad) => rad * 180 / Mathf.PI;

        private static float DegToRad(float deg) => deg * (Mathf.PI / 180);
    }
}