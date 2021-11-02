using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ZadanieRekrutacyjneIdeo.Data;
using ZadanieRekrutacyjneIdeo.Models;

namespace ZadanieRekrutacyjneIdeo.Repositories
{
    public class TreeNodesRepository : ITreeNodesRepository
    {
        private readonly ApplicationDbContext _context;

        public TreeNodesRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<TreeNode>> GetAllNodes()
        {
            var applicationDbContext = _context.TreeNodes.Include(t => t.Parent);
            return await applicationDbContext.ToListAsync();
        }
        public async Task<List<TreeNode>> GetNodesWithout(int? x)
        {
            var applicationDbContext = _context.TreeNodes.Include(t => t.Parent).Where(n => n.ID != x);
            return await applicationDbContext.ToListAsync();
        }

        public async Task AddTreeNode(TreeNode node)
        {
            await _context.TreeNodes.AddAsync(node);
            await _context.SaveChangesAsync();
        }

        public async Task<TreeNode> GetTreeNodeByIdAsync(int? id)
        {
            var node = await _context.TreeNodes.Include(t => t.Parent).FirstOrDefaultAsync(n => n.ID == id);
            return node;
        }
        public async Task EditTreeNode(TreeNode treeNode)
        {
            _context.TreeNodes.Update(treeNode);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAllNodes()
        {
            _context.TreeNodes.RemoveRange(_context.TreeNodes);
            await _context.SaveChangesAsync();
        }

        public bool TreeNodeExists(int id)
        {
            return _context.TreeNodes.Any(e => e.ID == id);
        }

        public async Task RemoveTreeNodeAsync(TreeNode node)
        {
            _context.TreeNodes.Remove(node);
            await _context.SaveChangesAsync();
        }
    }
}
