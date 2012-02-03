﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

//XNA imports
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace majorProject
{
    class Enemy
    {
        public int xPos;
        public int yPos;
        protected Texture2D sprite;
        public Rectangle hitBox;
        protected int spriteWidth;
        protected int spriteHeight;
        public bool alive = true;
        public int health;
        private double rotSpeed;
        public int rotAngle;
        public int aimTolerance;
        public double maxRotSpeed;

        public Enemy()
        {
        }
        /// <summary>
        /// Creates a new instance of an Enemy
        /// </summary>
        /// <param name="sprite"></param>
        /// <param name="spriteWidth"></param>
        /// <param name="spriteHeight"></param>
        /// <param name="xPos"></param>
        /// <param name="yPos"></param>
        public Enemy(Texture2D sprite, int spriteWidth, int spriteHeight, int xPos, int yPos, int health)
        {
            this.sprite = sprite;
            this.spriteHeight = spriteHeight;
            this.spriteWidth = spriteWidth;
            this.xPos = xPos;
            this.yPos = yPos;
            this.hitBox = new Rectangle(xPos, yPos, spriteWidth, spriteHeight);
            this.health = health;
            this.rotSpeed = 1;
            this.rotAngle = 180;
            this.aimTolerance = 0;
            this.maxRotSpeed = 0.1;
        }

        /// <summary>
        /// Updates the enemy's position, damage, and alive statues
        /// </summary>
        /// <param name="human"></param>
        public virtual void update(Player human)
        {
            foreach (Shot shot in human.shotList)
            {
                if (hitBox.Intersects(shot.hitBox))
                {
                    // subtract health from hit
                    health = health - shot.damage;

                    // if no health, set to dead
                    if (health <= 0)
                    {
                        alive = false;
                    }

                    // regester that the shot has connected
                    shot.hit = true;
                }
            }
        }
        /// <summary>
        /// Draws the enemy on to the screen
        /// </summary>
        /// <param name="batch"></param>
        public virtual void draw(SpriteBatch batch)
        {
            int drawx = xPos;
            int drawy = yPos;
            Rectangle drawrect = new Rectangle(drawx, drawy, spriteWidth, spriteHeight);
            batch.Draw(sprite, drawrect, Color.White);
        }

        public void die(Expolsion explosion, SpriteBatch batch)
        {
            explosion.xPos = xPos;
            explosion.yPos = yPos;
            //explosion.draw(batch);
        }

        public virtual float getAngleToHuman(Player human)
        {
            float angle = 0;
            //get angle to human need to fix
            int a = xPos + (spriteWidth / 2) - (human.xPos + human.sprite.spriteWidth / 2);
            int b = yPos + (spriteHeight / 2) - (human.yPos + human.sprite.spriteHeight / 2);

            double c = Math.Sqrt(Math.Pow(a, 2) + Math.Pow(b, 2));
            angle = (float)Math.Sin(a / c);

            //TODO find way to make angle transitions more smooth

            //if human is up and left of enemy
            if (human.xPos < xPos && human.yPos < yPos)
            {
                angle = angle + 90;
            }
            //if human is down and left of enemy
            else if (human.xPos > xPos && human.yPos < yPos)
            {
                angle = angle + 180;
            }
            else if (human.xPos > xPos && human.yPos > yPos)
            {
                angle = angle + 270;
            }

            
            return angle;
        }

        public bool moveTo(int tarX, int tarY)
        {
            bool finished = false;
            if (xPos > tarX)
            {
                xPos = xPos - 1;
            }
            else if (xPos < tarX)
            {
                xPos = xPos + 1;
            }

            if (yPos < tarY)
            {
                yPos = yPos + 1;
            }
            else if (yPos > tarY)
            {
                yPos = yPos - 1;
            }
            else
            {
                finished = true;
            }

            return finished;
        }

        /// <summary>
        /// Returns if the enemy is aimed within the tolerance 
        /// </summary>
        /// <param name="human"></param>
        /// <returns></returns>
        public bool isAimedAt(Player human, float humanAngle)
        {
            if (rotAngle <= humanAngle + aimTolerance || rotAngle >= humanAngle - aimTolerance)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool aim(Player human)
        {
            float humanAngle = getAngleToHuman(human);
            
            //if not aimed at human
            if (!isAimedAt(human, humanAngle))
            {
                if (rotAngle <= humanAngle)
                {
                    rotSpeed = maxRotSpeed;
                    return false;
                }
                else
                {
                    rotSpeed = -1 * maxRotSpeed;
                    return false;
                }
            }
            else
            {
                rotSpeed = 0;
                return true;
            }
        }
    }
}
