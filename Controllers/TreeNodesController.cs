using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ZadanieRekrutacyjneIdeo.Data;
using ZadanieRekrutacyjneIdeo.Models;
using ZadanieRekrutacyjneIdeo.Repositories;

namespace ZadanieRekrutacyjneIdeo.Controllers
{
    public class TreeNodesController : Controller
    {
        private readonly ITreeNodesRepository _treeNodesRepository;

        public TreeNodesController(ITreeNodesRepository treeNodesRepository)
        {

            _treeNodesRepository = treeNodesRepository;
        }

        // GET: TreeNodes
        public async Task<IActionResult> Index()
        {
            return View(await _treeNodesRepository.GetAllNodes());
        }

        //GET: TreeNodes/Details/5
            public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treeNode = await _treeNodesRepository.GetTreeNodeByIdAsync(id);
            if (treeNode == null)
            {
                return NotFound();
            }

            return View(treeNode);
        }

        // GET: TreeNodes/Create
        public async Task<IActionResult> Create()
        {
            List<TreeNode> nodes = await _treeNodesRepository.GetAllNodes();
            ViewData["PID"] = new SelectList(nodes, "ID", "Name");
            return View();
        }

        // POST: TreeNodes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Name,PID")] TreeNode treeNode)
        {
            if (ModelState.IsValid)
            {
                await _treeNodesRepository.AddTreeNode(treeNode);
                return RedirectToAction(nameof(Index));
            }
            ViewData["PID"] = new SelectList(await _treeNodesRepository.GetAllNodes(), "ID", "Name", treeNode.PID);
            return View(treeNode);
        }

        // GET: TreeNodes/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treeNode = await _treeNodesRepository.GetTreeNodeByIdAsync(id);
            if (treeNode == null)
            {
                return NotFound();
            }
            ViewData["PID"] = new SelectList(await _treeNodesRepository.GetNodesWithout(id), "ID", "Name", treeNode.PID);
            return View(treeNode);
        }

        // POST: TreeNodes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Name,PID")] TreeNode treeNode)
        {
            if (id != treeNode.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                   await _treeNodesRepository.EditTreeNode(treeNode);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_treeNodesRepository.TreeNodeExists(treeNode.ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["PID"] = new SelectList(await _treeNodesRepository.GetNodesWithout(id), "ID", "Name", treeNode.PID);
            return View(treeNode);
        }

        public async Task<IActionResult> RemoveAllTreeNodes()
        {
            await _treeNodesRepository.RemoveAllNodes();
            return RedirectToAction(nameof(Index));
        }

        // GET: TreeNodes/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treeNode = await _treeNodesRepository.GetTreeNodeByIdAsync(id);
            if (treeNode == null)
            {
                return NotFound();
            }

            return View(treeNode);
        }

        // POST: TreeNodes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var treeNode = await _treeNodesRepository.GetTreeNodeByIdAsync(id);
            await _treeNodesRepository.RemoveTreeNodeAsync(treeNode);
            return RedirectToAction(nameof(Index));
        }

    }
}
