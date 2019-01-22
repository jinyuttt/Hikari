//#region << 版 本 注 释 >>
///*----------------------------------------------------------------
//* 项目名称 ：Hikari.Integration.Models
//* 项目描述 ：
//* 类 名 称 ：Class1
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
//using System.Data;
//using System.Reflection;
//using System.Text;

//namespace Hikari.Integration.Models
//{
//    /* ============================================================================== 
//* 功能描述：Class1 
//* 创 建 者：jinyu 
//* 创建日期：2019 
//* 更新时间 ：2019
//* ==============================================================================*/

//    class Class1
//    {
//           #region Init Methods
   
//              private static Converter<IDataReader, List<T>> CreateBatchDataLoader(List<DbColumnInfo> columnInfoes)
//               {
//                  DynamicMethod dm = new DynamicMethod(string.Empty, typeof(List<T>),
//                      new Type[] { typeof(IDataReader) }, typeof(EntityConverter<T>));
//                  ILGenerator il = dm.GetILGenerator();
//                  LocalBuilder list = il.DeclareLocal(typeof(List<T>));
//                  LocalBuilder item = il.DeclareLocal(typeof(T));
//                 Label exit = il.DefineLabel();
//                  Label loop = il.DefineLabel();
//                  // List<T> list = new List<T>();
//                 il.Emit(OpCodes.Newobj, typeof(List<T>).GetConstructor(Type.EmptyTypes));
//                 il.Emit(OpCodes.Stloc_S, list);
//               // [ int %index% = arg.GetOrdinal(%ColumnName%); ]
//               LocalBuilder[] colIndices = GetColumnIndices(il, columnInfoes);
//                // while (arg.Read()) {
//                il.MarkLabel(loop);
//                 il.Emit(OpCodes.Ldarg_0);
//                 il.Emit(OpCodes.Callvirt, DataReader_Read);
//                 il.Emit(OpCodes.Brfalse, exit);
//                //      T item = new T { %Property% =  };
//                BuildItem(il, columnInfoes, item, colIndices);
//                 //      list.Add(item);
//                 il.Emit(OpCodes.Ldloc_S, list);
//                  il.Emit(OpCodes.Ldloc_S, item);
//                  il.Emit(OpCodes.Callvirt, typeof(List<T>).GetMethod("Add"));
//                 // }
//                il.Emit(OpCodes.Br, loop);
//                il.MarkLabel(exit);
//                // return list;
//                il.Emit(OpCodes.Ldloc_S, list);
//                il.Emit(OpCodes.Ret);
//                return (Converter<IDataReader, List<T>>) dm.CreateDelegate(typeof(Converter<IDataReader, List<T>>));
//             }
 
//            private static LocalBuilder[] GetColumnIndices(ILGenerator il, List<DbColumnInfo> columnInfoes)
//             {
//                 LocalBuilder[] colIndices = new LocalBuilder[columnInfoes.Count];
//                for (int i = 0; i<colIndices.Length; i++)
//                  {
//                     // int %index% = arg.GetOrdinal(%ColumnName%);
//                      colIndices[i] = il.DeclareLocal(typeof(int));
//                     il.Emit(OpCodes.Ldarg_0);
//                      il.Emit(OpCodes.Ldstr, columnInfoes[i].ColumnName);
//                    il.Emit(OpCodes.Callvirt, DataRecord_GetOrdinal);
//                     il.Emit(OpCodes.Stloc_S, colIndices[i]);
//                 }
//                  return colIndices;
//             }
 
//             private static void BuildItem(ILGenerator il, List<DbColumnInfo> columnInfoes,
//                 LocalBuilder item, LocalBuilder[] colIndices)
//            {
//                  // T item = new T();
//                il.Emit(OpCodes.Newobj, typeof(T).GetConstructor(Type.EmptyTypes));
//                 il.Emit(OpCodes.Stloc_S, item);
//                 for (int i = 0; i<colIndices.Length; i++)
//                  {
//                     if (IsCompatibleType(columnInfoes[i].Type, typeof(int)))
//                    {
//                        // item.%Property% = arg.GetInt32(%index%);
//                         ReadInt32(il, item, columnInfoes, colIndices, i);
//                     }
//                      else if (IsCompatibleType(columnInfoes[i].Type, typeof(int?)))
//                     {
//                       // item.%Property% = arg.IsDBNull ? default(int?) : (int?)arg.GetInt32(%index%);
//                        ReadNullableInt32(il, item, columnInfoes, colIndices, i);
//                     }
//                    else if (IsCompatibleType(columnInfoes[i].Type, typeof(long)))
//                      {
//                        // item.%Property% = arg.GetInt64(%index%);
//                        ReadInt64(il, item, columnInfoes, colIndices, i);
//                    }
//                     else if (IsCompatibleType(columnInfoes[i].Type, typeof(long?)))
//                      {
//                        // item.%Property% = arg.IsDBNull ? default(long?) : (long?)arg.GetInt64(%index%);
//                        ReadNullableInt64(il, item, columnInfoes, colIndices, i);
//                    }
//                     else if (IsCompatibleType(columnInfoes[i].Type, typeof(decimal)))
//                      {
//                        // item.%Property% = arg.GetDecimal(%index%);
//                        ReadDecimal(il, item, columnInfoes[i].SetMethod, colIndices[i]);
//                      }
//                    else if (columnInfoes[i].Type == typeof(decimal?))
//                   {
//                       // item.%Property% = arg.IsDBNull ? default(decimal?) : (int?)arg.GetDecimal(%index%);
//                         ReadNullableDecimal(il, item, columnInfoes[i].SetMethod, colIndices[i]);
//                     }
//                    else if (columnInfoes[i].Type == typeof(DateTime))
//                    {
//                         // item.%Property% = arg.GetDateTime(%index%);
//                        ReadDateTime(il, item, columnInfoes[i].SetMethod, colIndices[i]);
//                     }
//                    else if (columnInfoes[i].Type == typeof(DateTime?))
//                     {
//                       // item.%Property% = arg.IsDBNull ? default(DateTime?) : (int?)arg.GetDateTime(%index%);
//                        ReadNullableDateTime(il, item, columnInfoes[i].SetMethod, colIndices[i]);
//                    }
//                     else
//                     {
//                        // item.%Property% = (%PropertyType%)arg[%index%];
//                         ReadObject(il, item, columnInfoes, colIndices, i);
//                    }
//                 }
//            }
 
//             private static bool IsCompatibleType(Type t1, Type t2)
//             {
//                if (t1 == t2)
//                    return true;
//                if (t1.IsEnum && Enum.GetUnderlyingType(t1) == t2)
//                    return true;
//                 var u1 = Nullable.GetUnderlyingType(t1);
//                var u2 = Nullable.GetUnderlyingType(t2);
//                if (u1 != null && u2 != null)
//                    return IsCompatibleType(u1, u2);
//                 return false;
//            }

//            private static void ReadInt32(ILGenerator il, LocalBuilder item,
//                 List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
//             {
//                il.Emit(OpCodes.Ldloc_S, item);
//                il.Emit(OpCodes.Ldarg_0);
//                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
//                il.Emit(OpCodes.Callvirt, DataRecord_GetInt32);
//                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
//             }
 
//            private static void ReadNullableInt32(ILGenerator il, LocalBuilder item,
//                List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
//             {
//                 var local = il.DeclareLocal(columnInfoes[i].Type);
//                Label intNull = il.DefineLabel();
//                Label intCommon = il.DefineLabel();
//                 il.Emit(OpCodes.Ldloca, local);
//               il.Emit(OpCodes.Ldarg_0);
//                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
//                 il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
//                 il.Emit(OpCodes.Brtrue_S, intNull);
//                il.Emit(OpCodes.Ldarg_0);
//                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
//                il.Emit(OpCodes.Callvirt, DataRecord_GetInt32);
//               il.Emit(OpCodes.Call, columnInfoes[i].Type.GetConstructor(
//                    new Type[] { Nullable.GetUnderlyingType(columnInfoes[i].Type) }));
//                il.Emit(OpCodes.Br_S, intCommon);
//                il.MarkLabel(intNull);
//                il.Emit(OpCodes.Initobj, columnInfoes[i].Type);
//                il.MarkLabel(intCommon);
//                 il.Emit(OpCodes.Ldloc_S, item);
//                 il.Emit(OpCodes.Ldloc, local);
//                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
//            }
 
//             private static void ReadInt64(ILGenerator il, LocalBuilder item,
//                 List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
//            {
//                il.Emit(OpCodes.Ldloc_S, item);
//               il.Emit(OpCodes.Ldarg_0);
//                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
//                 il.Emit(OpCodes.Callvirt, DataRecord_GetInt64);
//                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
//            }

//             private static void ReadNullableInt64(ILGenerator il, LocalBuilder item,
//                List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
//            {
//                var local = il.DeclareLocal(columnInfoes[i].Type);
//                Label intNull = il.DefineLabel();
//                Label intCommon = il.DefineLabel();
//                 il.Emit(OpCodes.Ldloca, local);
//                il.Emit(OpCodes.Ldarg_0);
//                 il.Emit(OpCodes.Ldloc_S, colIndices[i]);
//                il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
//                il.Emit(OpCodes.Brtrue_S, intNull);
//                 il.Emit(OpCodes.Ldarg_0);
//                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
//                 il.Emit(OpCodes.Callvirt, DataRecord_GetInt64);
//                 il.Emit(OpCodes.Call, columnInfoes[i].Type.GetConstructor(
//                     new Type[] { Nullable.GetUnderlyingType(columnInfoes[i].Type) }));
//                il.Emit(OpCodes.Br_S, intCommon);
//                il.MarkLabel(intNull);
//                il.Emit(OpCodes.Initobj, columnInfoes[i].Type);
//                il.MarkLabel(intCommon);
//                il.Emit(OpCodes.Ldloc_S, item);
//                il.Emit(OpCodes.Ldloc, local);
//                il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
//             }

//             private static void ReadDecimal(ILGenerator il, LocalBuilder item,
//                 MethodInfo setMethod, LocalBuilder colIndex)
//             {
//                 il.Emit(OpCodes.Ldloc_S, item);
//                il.Emit(OpCodes.Ldarg_0);
//                il.Emit(OpCodes.Ldloc_S, colIndex);
//                il.Emit(OpCodes.Callvirt, DataRecord_GetDecimal);
//                il.Emit(OpCodes.Callvirt, setMethod);
//            }
 
//            private static void ReadNullableDecimal(ILGenerator il, LocalBuilder item,
//                MethodInfo setMethod, LocalBuilder colIndex)
//            {
//                var local = il.DeclareLocal(typeof(decimal?));
//                Label decimalNull = il.DefineLabel();
//                Label decimalCommon = il.DefineLabel();
//                 il.Emit(OpCodes.Ldloca, local);
//                il.Emit(OpCodes.Ldarg_0);
//                il.Emit(OpCodes.Ldloc_S, colIndex);
//                il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
//                il.Emit(OpCodes.Brtrue_S, decimalNull);
//                il.Emit(OpCodes.Ldarg_0);
//                il.Emit(OpCodes.Ldloc_S, colIndex);
//                 il.Emit(OpCodes.Callvirt, DataRecord_GetDecimal);
//                 il.Emit(OpCodes.Call, typeof(decimal?).GetConstructor(new Type[] { typeof(decimal) }));
//               il.Emit(OpCodes.Br_S, decimalCommon);
//               il.MarkLabel(decimalNull);
//               il.Emit(OpCodes.Initobj, typeof(decimal?));
//                il.MarkLabel(decimalCommon);
//                 il.Emit(OpCodes.Ldloc_S, item);
//                 il.Emit(OpCodes.Ldloc, local);
//                 il.Emit(OpCodes.Callvirt, setMethod);
//             }

//             private static void ReadDateTime(ILGenerator il, LocalBuilder item,
//                MethodInfo setMethod, LocalBuilder colIndex)
//             {
//                 il.Emit(OpCodes.Ldloc_S, item);
//                 il.Emit(OpCodes.Ldarg_0);
//                 il.Emit(OpCodes.Ldloc_S, colIndex);
//                 il.Emit(OpCodes.Callvirt, DataRecord_GetDateTime);
//                 il.Emit(OpCodes.Callvirt, setMethod);
//             }
 
//            private static void ReadNullableDateTime(ILGenerator il, LocalBuilder item,
//                MethodInfo setMethod, LocalBuilder colIndex)
//            {
//                 var local = il.DeclareLocal(typeof(DateTime?));
//                Label dtNull = il.DefineLabel();
//                 Label dtCommon = il.DefineLabel();
//                 il.Emit(OpCodes.Ldloca, local);
//                il.Emit(OpCodes.Ldarg_0);
//                 il.Emit(OpCodes.Ldloc_S, colIndex);
//                il.Emit(OpCodes.Callvirt, DataRecord_IsDBNull);
//                il.Emit(OpCodes.Brtrue_S, dtNull);
//                 il.Emit(OpCodes.Ldarg_0);
//                il.Emit(OpCodes.Ldloc_S, colIndex);
//                il.Emit(OpCodes.Callvirt, DataRecord_GetDateTime);
//                 il.Emit(OpCodes.Call, typeof(DateTime?).GetConstructor(new Type[] { typeof(DateTime) }));
//                 il.Emit(OpCodes.Br_S, dtCommon);
//                il.MarkLabel(dtNull);
//                 il.Emit(OpCodes.Initobj, typeof(DateTime?));
//                il.MarkLabel(dtCommon);
//                il.Emit(OpCodes.Ldloc_S, item);
//                il.Emit(OpCodes.Ldloc, local);
//                il.Emit(OpCodes.Callvirt, setMethod);
//             }
 
//            private static void ReadObject(ILGenerator il, LocalBuilder item,
//                 List<DbColumnInfo> columnInfoes, LocalBuilder[] colIndices, int i)
//             {
//                 Label common = il.DefineLabel();
//                 il.Emit(OpCodes.Ldloc_S, item);
//                il.Emit(OpCodes.Ldarg_0);
//                il.Emit(OpCodes.Ldloc_S, colIndices[i]);
//                il.Emit(OpCodes.Callvirt, DataRecord_ItemGetter_Int);
//                 il.Emit(OpCodes.Dup);
//                il.Emit(OpCodes.Call, Convert_IsDBNull);
//               il.Emit(OpCodes.Brfalse_S, common);
//                il.Emit(OpCodes.Pop);
//                 il.Emit(OpCodes.Ldnull);
//                il.MarkLabel(common);
//                 il.Emit(OpCodes.Unbox_Any, columnInfoes[i].Type);
//                 il.Emit(OpCodes.Callvirt, columnInfoes[i].SetMethod);
//             }

//             #endregion

//            #region Internal Methods

//             internal static List<T> Select(IDataReader reader)
//            {
//                return BatchDataLoader(reader);
//            }
 
//            #endregion
//    }
//}
