﻿using System;
using UnityEngine;

namespace Chaos
{

    public class Packer
    {
        public struct Block
        {
            public int x;
            public int y;
            public int w;
            public int h;
            public int i;

            public static Block operator *(Block b, float f)
            {
                b.x = (int) (b.x*f);
                b.y = (int)(b.y * f);
                b.w = (int)(b.w * f);
                b.h = (int)(b.h * f);
                return b;
            }
        }

        private class Node
        {
            public int w;
            public int h;
            public int x;
            public int y;
            public Node right;
            public Node down;
            public bool used;
        }
        private Node _root = new Node();

        public void Fit(Block[] blocks, out int width, out int height)
        {
            int len = blocks.Length;
            if (null == blocks || 0 == len)
            {
                throw new Exception("Array underflow");
            }
            width = _root.w = blocks[0].w;
            height = _root.h = blocks[0].h;
            for (int n = 0; n < len; n++)
            {
                Block block = blocks[n];
                Node node = FindNode(_root, block.w, block.h);
                if (node != null)
                {
                    node = SplitNode(node, block.w, block.h);
                }
                else
                {
                    node = GrowNode(block.w, block.h);
                }
                block.x = node.x;
                block.y = node.y;
                blocks[n] = block;
                width = Math.Max(width, block.x + block.w);
                height = Math.Max(height, block.y + block.h);
            }
        }

        private Node FindNode(Node root, int w, int h)
        {
            if (root.used)
            {
                return FindNode(root.right, w, h) ?? FindNode(root.down, w, h);
            }
            if (w <= root.w && h <= root.h)
            {
                return root;
            }
            return null;
        }
        private Node SplitNode(Node node, int w, int h)
        {
            node.used = true;
            node.down = new Node() { x = node.x, y = node.y + h, w = node.w, h = node.h - h, };
            node.right = new Node() { x = node.x + w, y = node.y, w = node.w - w, h = h, };
            return node;
        }

        private Node GrowNode(int w, int h)
        {
            bool canGrowDown = w <= _root.w;
            bool canGrowRight = h <= _root.h;
            if (canGrowRight && _root.h >= _root.w + w)
            {
                return GrowRight(w, h);
            }
            if (canGrowDown && _root.w >= _root.h + h)
            {
                return GrowDown(w, h);
            }
            if (canGrowRight)
            {
                return GrowRight(w, h);
            }
            if (canGrowDown)
            {
                return GrowDown(w, h);
            }
            return null;
        }

        private Node GrowRight(int w, int h)
        {
            _root = new Node(){
                used = true,
                x = 0,
                y = 0,
                w = _root.w + w,
                h = _root.h,
                down = _root,
                right = new Node() { x = _root.w, y = 0, w = w, h = _root.h },
            };
            Node node = FindNode(_root, w, h);
            return node == null ? null : SplitNode(node, w, h);
        }
        private Node GrowDown(int w, int h)
        {
            _root = new Node(){
                used = true,
                x = 0,
                y = 0,
                w = _root.w,
                h = _root.h + h,
                right = _root,
                down = new Node(){ x = 0, y = _root.h, w = _root.w, h = h },
            };
            Node node = FindNode(_root, w, h);
            return node == null ? null : SplitNode(node, w, h);
        }
    }
}