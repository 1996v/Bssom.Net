using System.Collections.Generic;

namespace BssomSerializers
{
    /// <summary>
    /// <para>用于在上下文中存储和读取自定义的数据</para>
    /// <para>Used to store and read data taken from definitions in context</para>
    /// </summary>
    public struct ContextDataSlots
    {
        private Dictionary<string, object> storeSlots;

        /// <summary>
        /// <para>尝试从当前上下文中的槽中获取指定名称的数据</para>
        /// <para> attempts to get the data of the specified name from the slot in the current context </para >
        /// </summary>
        /// <param name="name">插槽的名称. The name of the local data slot.</param>
        /// <param name="data">存放在槽中的数据. data stored in a slot</param>
        /// <returns>如果找到则返回true. Returns true if found</returns>
        public bool TryGetNamedDataSlot(string name, out object data)
        {
            if (storeSlots != null && storeSlots.TryGetValue(name, out data))
            {
                return true;
            }

            data = default;
            return false;
        }

        /// <summary>
        /// <para>在当前上下文中分配命名的数据插槽</para>
        /// <para>Assign named data slots in the current context</para>
        /// </summary>
        /// <param name="name">插槽的名称. The name of the slot</param>
        /// <param name="data">存放在槽中的数据. data stored in a slot</param>
        /// <exception cref="System.ArgumentException">如果存在相同名称的插槽,则产生异常. If there is a slot with the same name, an exception is generated</exception>
        public void AllocateNamedDataSlot(string name, object data)
        {
            if (storeSlots == null)
                storeSlots = new Dictionary<string, object>();

            storeSlots.Add(name, data);
        }

        /// <summary>
        /// <para>设置当前上下文中的指定槽中的数据</para>
        /// <para>Sets the data in the specified slot in the current context</para>
        /// </summary>
        /// <param name="name">要设置值的槽的名称. The name of the slot to set the value</param>
        /// <param name="data">要设置的数据. The data to set</param>
        /// <exception cref="System.Collections.Generic.KeyNotFoundException">如果该槽并不存在,则产生异常. If the slot does not exist, an exception is generated</exception>
        public void SetNamedDataSlot(string name, object data)
        {
            storeSlots[name] = data;
        }

        /// <summary>
        /// <para>删除指定名称的数据槽</para>
        /// <para>Deletes a data slot with the specified name</para>
        /// </summary>
        /// <returns>如果成功找到并删除则返回true. Returns true if found and deleted successfully</returns>
        public bool RemoveNamedDataSlot(string name)
        {
            if (storeSlots != null)
                return storeSlots.Remove(name);
            return false;
        }

        /// <summary>
        /// <para>清除所有的槽</para>
        /// <para>Clear all slots</para>
        /// </summary>
        public void ClearSlots()
        {
            if (storeSlots != null)
                storeSlots.Clear();
        }
    }
}