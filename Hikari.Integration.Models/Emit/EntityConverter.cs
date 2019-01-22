//#region << 版 本 注 释 >>
///*----------------------------------------------------------------
//* 项目名称 ：Hikari.Integration.Models
//* 项目描述 ：
//* 类 名 称 ：EntityConverter
//* 类 描 述 ：
//* 命名空间 ：Hikari.Integration.Models
//* CLR 版本 ：4.0.30319.42000
//* 作    者 ：jinyu
//* 创建时间 ：2019
//* 版 本 号 ：v1.0.0.0
//*******************************************************************
//* Copyright @ jinyu 2019. All rights reserved.
//*******************************************************************
////----------------------------------------------------------------*/
//#endregion



//using System;
//using System.Collections.Generic;
//using System.Text;
//using System.Data;
//using System.Reflection;
//using System.Reflection.Emit;
//using System.Collections.Concurrent;

//namespace Hikari.Integration.Models
//{
//    /* ============================================================================== 
//* 功能描述：EntityConverter Emit转换实体
//* 创 建 者：jinyu 
//* 创建日期：2019 
//* 更新时间 ：2019
//* ==============================================================================*/

//  public static  class EntityConverter
//    {
//        private static ConcurrentDictionary<string, object> cache = new ConcurrentDictionary<string, object>();
//        //数据类型和对应的强制转换方法的methodinfo，供实体属性赋值时调用
//        private static Dictionary<Type, MethodInfo> ConvertMethods = new Dictionary<Type, MethodInfo>()
//       {
//           {typeof(int),typeof(Convert).GetMethod("ToInt32",new Type[]{typeof(object)})},
//           {typeof(Int16),typeof(Convert).GetMethod("ToInt16",new Type[]{typeof(object)})},
//           {typeof(Int64),typeof(Convert).GetMethod("ToInt64",new Type[]{typeof(object)})},
//           {typeof(DateTime),typeof(Convert).GetMethod("ToDateTime",new Type[]{typeof(object)})},
//           {typeof(decimal),typeof(Convert).GetMethod("ToDecimal",new Type[]{typeof(object)})},
//           {typeof(Double),typeof(Convert).GetMethod("ToDouble",new Type[]{typeof(object)})},
//           {typeof(Boolean),typeof(Convert).GetMethod("ToBoolean",new Type[]{typeof(object)})},
//           {typeof(string),typeof(Convert).GetMethod("ToString",new Type[]{typeof(object)})}
//       };

//        //把datarow转换为实体的方法的委托定义
//        public delegate T LoadDataRow<T>(DataRow dr);
//        //把datareader转换为实体的方法的委托定义
//        public delegate T LoadDataRecord<T>(IDataRecord dr);

//        //emit里面用到的针对datarow的元数据信息
//        private static readonly AssembleInfo dataRowAssembly = new AssembleInfo(typeof(DataRow));
//        //emit里面用到的针对datareader的元数据信息
//        private static readonly AssembleInfo dataRecordAssembly = new AssembleInfo(typeof(IDataRecord));

//        /// <summary>
//        /// 构造转换动态方法（核心代码），根据assembly可处理datarow和datareader两种转换
//        /// </summary>
//        /// <typeparam name="T">返回的实体类型</typeparam>
//        /// <param name="assembly">待转换数据的元数据信息</param>
//        /// <returns>实体对象</returns>
//        private static DynamicMethod BuildMethod<T>(AssembleInfo assembly)
//        {
//            DynamicMethod method = new DynamicMethod(assembly.MethodName + typeof(T).Name, MethodAttributes.Public | MethodAttributes.Static, CallingConventions.Standard, typeof(T),
//                    new Type[] { assembly.SourceType }, typeof(EntityContext).Module, true);
//            ILGenerator generator = method.GetILGenerator();
//            LocalBuilder result = generator.DeclareLocal(typeof(T));
//            generator.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
//            generator.Emit(OpCodes.Stloc, result);

//            foreach (PropertyInfo property in typeof(T).GetProperties())
//            {
//                var endIfLabel = generator.DefineLabel();
//                generator.Emit(OpCodes.Ldarg_0);
//                generator.Emit(OpCodes.Ldstr, property.Name);
//                generator.Emit(OpCodes.Call, assembly.CanSettedMethod);
//                generator.Emit(OpCodes.Brfalse, endIfLabel);
//                generator.Emit(OpCodes.Ldloc, result);
//                generator.Emit(OpCodes.Ldarg_0);
//                generator.Emit(OpCodes.Ldstr, property.Name);
//                generator.Emit(OpCodes.Callvirt, assembly.GetValueMethod);
//                if (property.PropertyType.IsValueType || property.PropertyType == typeof(string))
//                    generator.Emit(OpCodes.Call, ConvertMethods[property.PropertyType]);
//                else
//                    generator.Emit(OpCodes.Castclass, property.PropertyType);
//                generator.Emit(OpCodes.Call, property.GetSetMethod());
//                generator.MarkLabel(endIfLabel);
//            }
//            generator.Emit(OpCodes.Ldloc, result);
//            generator.Emit(OpCodes.Ret);
//            return method;
//        }

//        /// <summary>
//        /// 从Cache获取委托 LoadDataRow<T>的方法实例，没有则调用BuildMethod构造一个。
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <returns></returns>
//        private static LoadDataRow<T> GetDataRowMethod<T>()
//        {
//            string key = dataRowAssembly.MethodName + typeof(T).Name;
//            LoadDataRow<T> load = null;
//            object v = null;
//            if(cache.TryGetValue(key,out v))
//            {
//                load = v as LoadDataRow<T>;
//            }
//            else
//            {
//                load = (LoadDataRow<T>)BuildMethod<T>(dataRowAssembly).CreateDelegate(typeof(LoadDataRow<T>));
//                cache[key] = load;
//            }
           
//            return load;
//        }

//        /// <summary>
//        /// 从Cache获取委托 LoadDataRecord<T>的方法实例，没有则调用BuildMethod构造一个。
//        /// </summary>
//        /// <typeparam name="T"></typeparam>
//        /// <returns></returns>
//        private static LoadDataRecord<T> GetDataRecordMethod<T>()
//        {
//            string key = dataRecordAssembly.MethodName + typeof(T).Name;
//            LoadDataRecord<T> load = null;
//            object v = null;
//            if (cache.TryGetValue(key, out v))
//            {
//                load = v as LoadDataRecord<T>;
//            }
//            else
//            {
//                load = (LoadDataRecord<T>)BuildMethod<T>(dataRecordAssembly).CreateDelegate(typeof(LoadDataRecord<T>));
//                cache[key] = load;
//            }
           
//            return load;
//        }


//        public static T ToItem<T>(DataRow dr)
//        {
//            LoadDataRow<T> load = GetDataRowMethod<T>();
//            return load(dr);
//        }

//        public static List<T> ToList<T>(DataTable dt)
//        {
//            List<T> list = new List<T>();
//            if (dt == null || dt.Rows.Count == 0)
//            {
//                return list;
//            }
//            LoadDataRow<T> load = GetDataRowMethod<T>();
//            foreach (DataRow dr in dt.Rows)
//            {
//                list.Add(load(dr));
//            }
//            return list;
//        }

//        public static List<T> ToList<T>(IDataReader dr)
//        {
//            List<T> list = new List<T>();
//            LoadDataRecord<T> load = GetDataRecordMethod<T>();
//            while (dr.Read())
//            {
//                list.Add(load(dr));
//            }
//            return list;
//        }

//    }
//}
